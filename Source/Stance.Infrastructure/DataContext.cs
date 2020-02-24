// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stance.Core.Contracts.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;

namespace Stance.Infrastructure
{
    public sealed class DataContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;

        public DataContext(DbContextOptions<DataContext> options, IMediator mediator)
            : base(options)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public DbSet<User> Users { get; set; }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await this.SaveChangesAsync(cancellationToken);
            await this._mediator.DispatchDomainEventsAsync(this);
            return true;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(this.ConfigureUser);
        }

        private void ConfigureUser(EntityTypeBuilder<User> users)
        {
            users.ToTable("user", "identity");
            users.HasKey(entity => entity.Id);
            users.Ignore(b => b.DomainEvents);
            users.Property(e => e.Id).ValueGeneratedNever();

            var navigation = users.Metadata.FindNavigation(nameof(User.AuthenticationHistories));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            users.OwnsMany<AuthenticationHistory>(user => user.AuthenticationHistories, authenticationHistories =>
            {
                authenticationHistories.ToTable("authenticationHistory", "identity");
                authenticationHistories.HasKey(authenticationHistory => authenticationHistory.Id);
                authenticationHistories.Property(authenticationHistory => authenticationHistory.Id)
                    .ValueGeneratedNever();
                authenticationHistories.Ignore(authenticationHistory => authenticationHistory.DomainEvents);
            });

            navigation = users.Metadata.FindNavigation(nameof(User.SecurityTokenMappings));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            users.OwnsMany<SecurityTokenMapping>(user => user.SecurityTokenMappings, securityTokenMappings =>
            {
                securityTokenMappings.ToTable("securityTokenMapping", "identity");
                securityTokenMappings.HasKey(securityTokenMapping => securityTokenMapping.Id);
                securityTokenMappings.Property(securityTokenMapping => securityTokenMapping.Id)
                    .ValueGeneratedNever();
                securityTokenMappings.Ignore(securityTokenMapping => securityTokenMapping.DomainEvents);
            });

            users.OwnsOne(user => user.Profile, profile =>
            {
                profile.ToTable("profile", "identity");
                profile.WithOwner().HasForeignKey(x => x.Id);
                profile.HasKey(e => e.Id);
                profile.Property(x => x.Id).HasColumnName("UserId");
                profile.Ignore(b => b.DomainEvents);
            });
        }
    }
}