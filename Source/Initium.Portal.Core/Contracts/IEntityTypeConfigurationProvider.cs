using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Core.Contracts
{
    public interface IEntityTypeConfigurationProvider
    {
        void ApplyConfigurations(ModelBuilder modelBuilder);
    }
}