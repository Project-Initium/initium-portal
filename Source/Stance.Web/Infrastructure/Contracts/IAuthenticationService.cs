using System;
using System.Threading.Tasks;

namespace Stance.Web.Infrastructure.Contracts
{
    public interface IAuthenticationService
    {
        Task SignInUserAsync(Guid userId);

        Task SignInUserPartiallyAsync(Guid userId, string returnUrl = null);

        Task<string> SignInUserFromPartialStateAsync(Guid userId);
    }
}