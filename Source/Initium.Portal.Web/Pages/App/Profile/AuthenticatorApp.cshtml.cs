// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using FluentValidation;
using Initium.Portal.Core;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Queries.Contracts.Static;
using Initium.Portal.Web.Infrastructure.PageModels;
using Microsoft.AspNetCore.Authorization;

namespace Initium.Portal.Web.Pages.App.Profile
{
    [Authorize]
    public class AuthenticatorApp : PrgPageModel<AuthenticatorApp.Model>
    {
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;

        private readonly IUserQueries _userQueries;

        public AuthenticatorApp(
            IUserQueries userQueries,
            ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ??
                                                     throw new ArgumentNullException(
                                                         nameof(currentAuthenticatedUserProvider));
            this._userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
        }

        public bool IsSetup { get; private set; }

        public async Task OnGetAsync()
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                this.PrgState = PrgState.InError;
                return;
            }

            if (currentUserMaybe.Value is AuthenticatedUser)
            {
                var userCheck = await this._userQueries.CheckForPresenceOfAuthAppForCurrentUser();

                if (userCheck.IsPresent)
                {
                    this.IsSetup = true;
                }
            }
            else
            {
                this.PrgState = PrgState.InError;
            }
        }

        public class Model
        {
            public string Code { get; set; }

            public string Password { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.Code)
                    .NotEmpty().WithMessage("Please enter the code from your authenticator app.");
                this.RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Please enter your password.");
            }
        }
    }
}