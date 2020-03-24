// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stance.Core;
using Stance.Core.Constants;
using Stance.Core.Contracts;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Infrastructure.Constants;
using Stance.Web.Infrastructure.Contracts;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.Auth
{
    [Authorize(AuthenticationSchemes = "login-partial")]
    public class ValidateEmailMfaCode : PrgPageModel<ValidateEmailMfaCode.Model>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMediator _mediator;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;

        public ValidateEmailMfaCode(IMediator mediator, IAuthenticationService authenticationService, ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._authenticationService =
                authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
        }

        public bool HasApp { get; set; }

        public bool HasDevice { get; set; }

        public void OnGet()
        {
            var maybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (maybe.HasValue && maybe.Value is UnauthenticatedUser user)
            {
                this.HasApp = user.SetupMfaProviders.HasFlag(MfaProvider.App);
                this.HasDevice = user.SetupMfaProviders.HasFlag(MfaProvider.Device);
            }
        }

        public async Task<IActionResult> OnPost()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result =
                await this._mediator.Send(new ValidateEmailMfaCodeAgainstCurrentUserCommand(this.PageModel.Code));
            if (result.IsSuccess)
            {
                var returnUrl = await this._authenticationService.SignInUserFromPartialStateAsync(result.Value.UserId);

                if (string.IsNullOrEmpty(returnUrl))
                {
                    return this.RedirectToPage(PageLocations.AppDashboard);
                }

                return this.LocalRedirect(returnUrl);
            }

            this.PrgState = PrgState.InError;
            return this.RedirectToPage();
        }

        public async Task<IActionResult> OnPostAppMfaAsync()
        {
            var result =
                await this._mediator.Send(new AppMfaRequestedCommand());

            if (result.IsSuccess)
            {
                return this.RedirectToPage(PageLocations.AuthAppMfa);
            }

            this.PrgState = PrgState.InError;
            return this.RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeviceMfaAsync()
        {
            var result =
                await this._mediator.Send(new DeviceMfaRequestCommand());

            if (result.IsSuccess)
            {
                this.TempData["fido2.assertionOptions"] = result.Value.AssertionOptions.ToJson();
                return this.RedirectToPage(PageLocations.AuthDeviceMfa);
            }

            this.PrgState = PrgState.InError;
            return this.RedirectToPage();
        }

        public class Model
        {
            public string Code { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.Code).NotEmpty();
            }
        }
    }
}