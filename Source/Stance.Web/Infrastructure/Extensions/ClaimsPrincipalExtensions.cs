// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Security.Claims;

namespace Stance.Web.Infrastructure.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool HasPermissions(this ClaimsPrincipal user, string resource)
        {
            return user.HasClaim(x => x.Type == ClaimTypes.System) || user.HasClaim(ClaimTypes.Role, resource);
        }
    }
}