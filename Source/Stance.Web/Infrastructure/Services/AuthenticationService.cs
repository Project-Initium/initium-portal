// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Stance.Core.Constants;
using Stance.Queries.Contracts;
using IAuthenticationService = Stance.Web.Infrastructure.Contracts.IAuthenticationService;

namespace Stance.Web.Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserQueries _userQueries;

        public AuthenticationService(IHttpContextAccessor httpContextAccessor, IUserQueries userQueries)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._userQueries = userQueries;
        }

        public async Task SignInUserAsync(Guid userId)
        {
            var maybe = await this._userQueries.GetSystemProfileByUserId(userId);
            if (maybe.HasNoValue)
            {
                throw new InvalidOperationException();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Upn, userId.ToString()),
                new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(
                    new UserProfile(
                        userId,
                        maybe.Value.EmailAddress,
                        maybe.Value.FirstName,
                        maybe.Value.LastName,
                        maybe.Value.IsAdmin,
                        maybe.Value.Resources.ToList()), Formatting.None)),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties();

            await this._httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        public async Task SignInUserPartiallyAsync(
            Guid userId,
            MfaProvider setupMfaProviders,
            string returnUrl = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Anonymous, JsonConvert.SerializeObject(new AuthenticationProfile(userId, setupMfaProviders))),
            };

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                claims.Add(new Claim(ClaimTypes.UserData, returnUrl));
            }

            await this._httpContextAccessor.HttpContext.SignInAsync(
                "login-partial",
                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
        }

        public async Task<string> SignInUserFromPartialStateAsync(Guid userId)
        {
            var returnUrl = this._httpContextAccessor.HttpContext.User.HasClaim(x => x.Type == ClaimTypes.UserData)
                ? this._httpContextAccessor.HttpContext.User.Claims.First(x => x.Type == ClaimTypes.UserData).Value
                : string.Empty;
            await this._httpContextAccessor.HttpContext.SignOutAsync("login-partial");

            await this.SignInUserAsync(userId);

            return returnUrl;
        }

        internal class AuthenticationProfile
        {
            [JsonConstructor]
            public AuthenticationProfile(Guid userId, MfaProvider setupMfaProviders)
            {
                this.UserId = userId;
                this.SetupMfaProviders = setupMfaProviders;
            }

            public Guid UserId { get; }

            public MfaProvider SetupMfaProviders { get; }
        }

        internal class UserProfile
        {
            [JsonConstructor]
            public UserProfile(Guid userId, string emailAddress, string firstName, string lastName, bool isAdmin,
                List<string> resources)
            {
                this.Resources = resources;
                this.UserId = userId;
                this.EmailAddress = emailAddress;
                this.FirstName = firstName;
                this.LastName = lastName;
                this.IsAdmin = isAdmin;
            }

            public Guid UserId { get; }

            public string EmailAddress { get; }

            public string FirstName { get; }

            public string LastName { get; }

            public bool IsAdmin { get; }

            public IReadOnlyList<string> Resources { get; }
        }
    }
}