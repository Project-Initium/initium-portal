// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stance.Queries.OData.Entities;

namespace Stance.Queries.OData
{
    public sealed class ODataContext : DbContext
    {
        public ODataContext(DbContextOptions<ODataContext> options)
            : base(options)
        {
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(this.ConfigureUser);
            modelBuilder.Entity<Role>(this.ConfigureRole);
        }

        private void ConfigureRole(EntityTypeBuilder<Role> config)
        {
            config.ToTable("vwRole", "ReadAggregation");
            config.HasKey(x => x.Id);
        }

        private void ConfigureUser(EntityTypeBuilder<User> config)
        {
            config.ToTable("vwUser", "ReadAggregation");
            config.HasKey(x => x.Id);
        }
    }
}