// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Security.Claims;
using Initium.Portal.Core.Authentication;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Web.Infrastructure.Services;
using MaybeMonad;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Initium.Portal.Web.Infrastructure
{
    public class CurrentAuthenticatedUserProvider : ICurrentAuthenticatedUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentAuthenticatedUserProvider(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public Maybe<ISystemUser> CurrentAuthenticatedUser
        {
            get
            {
                if (!this._httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    return Maybe<ISystemUser>.Nothing;
                }

                if (this._httpContextAccessor.HttpContext.User.HasClaim(x => x.Type == ClaimTypes.Anonymous))
                {
                    var profileJson = this._httpContextAccessor.HttpContext.User.Claims
                        .First(x => x.Type == ClaimTypes.Anonymous).Value;
                    var profile = JsonConvert.DeserializeObject<AuthenticationService.AuthenticationProfile>(profileJson);
                    ISystemUser user = new UnauthenticatedUser(profile.UserId, profile.SetupMfaProviders);
                    return Maybe.From(user);
                }

                if (this._httpContextAccessor.HttpContext.User.HasClaim(x => x.Type == ClaimTypes.Upn))
                {
                    var profileJson = this._httpContextAccessor.HttpContext.User.Claims
                        .First(x => x.Type == ClaimTypes.UserData).Value;

                    var profile = JsonConvert.DeserializeObject<AuthenticationService.UserProfile>(profileJson);

                    ISystemUser user = new AuthenticatedUser(profile.UserId, profile.EmailAddress, profile.FirstName,
                        profile.LastName);
                    return Maybe.From(user);
                }

                return Maybe<ISystemUser>.Nothing;
            }
        }
    }
}