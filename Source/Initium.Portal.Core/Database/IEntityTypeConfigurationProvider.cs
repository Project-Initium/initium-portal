using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Core.Database
{
    public interface IEntityTypeConfigurationProvider
    {
        void ApplyConfigurations(ModelBuilder modelBuilder);
    }
}