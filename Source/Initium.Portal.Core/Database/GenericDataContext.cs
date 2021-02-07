using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Contracts.Queries;
using Initium.Portal.Core.Exceptions;
using Initium.Portal.Core.Extensions;
using Initium.Portal.Core.MultiTenant;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Core.Database
{
    public class GenericDataContext : DbContext, IUnitOfWork
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly FeatureBasedTenantInfo _tenantInfo;

        public GenericDataContext(DbContextOptions<GenericDataContext> options, IServiceProvider serviceProvider, FeatureBasedTenantInfo tenantInfo)
            : base(options)
        {
            this._serviceProvider = serviceProvider;
            this._tenantInfo = tenantInfo;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                return;
            }

            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(this._tenantInfo.ConnectionString);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var mediator = this.GetService<IMediator>();
            await mediator.DispatchDomainEventsAsync(this);
            await this.SaveChangesAsync(cancellationToken);
            await mediator.DispatchIntegrationEventsAsync(this);
            return true;
        }
        
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (this.ChangeTracker.Entries().Any(entityEntry => entityEntry.Entity is IReadOnlyEntity))
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