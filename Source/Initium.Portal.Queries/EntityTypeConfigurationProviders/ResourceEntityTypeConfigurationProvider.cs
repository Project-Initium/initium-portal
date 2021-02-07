using Initium.Portal.Core.Database;
using Initium.Portal.Queries.Entities;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries.EntityTypeConfigurationProviders
{
    public class ResourceEntityTypeConfigurationProvider : IEntityTypeConfigurationProvider
    {
        public void ApplyConfigurations(ModelBuilder modelBuilder)
        {
            var resources = modelBuilder.Entity<ResourceReadEntity>();

            resources.ToTable("vwResource", "Portal");
            resources.HasKey(resource => resource.Id);
        }
    }
}