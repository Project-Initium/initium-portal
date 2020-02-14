// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Security.Claims;
using MaybeMonad;
using Microsoft.AspNetCore.Http;
using Stance.Core;
using Stance.Core.Contracts;

namespace Stance.Web.Infrastructure
{
    public class CurrentAuthenticatedUserProvider : ICurrentAuthenticatedUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentAuthenticatedUserProvider(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public Maybe<AuthenticatedUser> CurrentAuthenticatedUser
        {
            get
            {
                if (!this._httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    return Maybe<AuthenticatedUser>.Nothing;
                }

                if (!this._httpContextAccessor.HttpContext.User.HasClaim(x => x.Type == ClaimTypes.Upn))
                {
                    return Maybe<AuthenticatedUser>.Nothing;
                }

                var userId = Guid.Parse(this._httpContextAccessor.HttpContext.User.Claims
                    .First(x => x.Type == ClaimTypes.Upn).Value);

                return Maybe.From(new AuthenticatedUser(userId));
            }
        }
    }
}