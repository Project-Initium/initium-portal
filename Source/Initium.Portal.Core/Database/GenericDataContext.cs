// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Common;
using EntityFramework.Exceptions.SqlServer;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Contracts.Queries;
using Initium.Portal.Core.Exceptions;
using Initium.Portal.Core.Extensions;
using Initium.Portal.Core.MultiTenant;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ResultMonad;

[assembly:InternalsVisibleTo("Initium.Portal.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Initium.Portal.Core.Database
{
    public class GenericDataContext : DbContext, IUnitOfWork
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly FeatureBasedTenantInfo _tenantInfo;
        private readonly IMediator _mediator;

        public GenericDataContext(IServiceProvider serviceProvider, FeatureBasedTenantInfo tenantInfo, IMediator mediator)
        {
            this._serviceProvider = serviceProvider;
            this._tenantInfo = tenantInfo;
            this._mediator = mediator;
        }

        internal GenericDataContext(DbContextOptions<GenericDataContext> options, IServiceProvider serviceProvider,
            FeatureBasedTenantInfo tenantInfo, IMediator mediator)
            : base(options)
        {
            this._serviceProvider = serviceProvider;
            this._tenantInfo = tenantInfo;
            this._mediator = mediator;
        }

        public async Task<ResultWithError<IPersistenceError>> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await this._mediator.DispatchDomainEventsAsync(this);
            try
            {
                await this.SaveChangesAsync(cancellationToken);
            }
            catch (UniqueConstraintException)
            {
                return ResultWithError.Fail<IPersistenceError>(new UniquePersistenceError());
            }
            catch (ReferenceConstraintException)
            {
                return ResultWithError.Fail<IPersistenceError>(new InUsePersistenceError());
            }

            await this._mediator.DispatchIntegrationEventsAsync(this);
            return ResultWithError.Ok<IPersistenceError>();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (this.ChangeTracker.Entries().Any(entityEntry => entityEntry.Entity is ReadOnlyEntity))
            {
                throw new CustomException("Trying to save read only entity");
            }

            foreach (var entry in this.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added && entry.Metadata.FindAnnotation("MULTI_TENANT") != null)
                {
                    entry.Property("TenantId").CurrentValue = Guid.Parse(this._tenantInfo.Id);
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                return;
            }

            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseExceptionProcessor();
            optionsBuilder.UseSqlServer(this._tenantInfo.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var providers = this._serviceProvider.GetServices<IEntityTypeConfigurationProvider>();
            foreach (var entityTypeConfigurationProvider in providers)
            {
                entityTypeConfigurationProvider.ApplyConfigurations(modelBuilder);
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}