// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using FluentValidation;
using Initium.Portal.Core;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.Contracts;
using Initium.Portal.Web.Infrastructure.PageModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Pages.Auth
{
    [Authorize(AuthenticationSchemes = "login-partial")]
    public class ValidateAppMfaCode : PrgPageModel<ValidateAppMfaCode.Model>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMediator _mediator;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;

        public ValidateAppMfaCode(IMediator mediator, IAuthenticationService authenticationService, ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            this._mediator = mediator;
            this._authenticationService = authenticationService;
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
        }

        public bool HasDevice { get; set; }

        public void OnGet()
        {
            var maybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (maybe.HasValue && maybe.Value is UnauthenticatedUser user)
            {
                this.HasDevice = user.SetupMfaProviders.HasFlag(MfaProvider.Device);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result =
                await this._mediator.Send(new ValidateAppMfaCodeAgainstCurrentUserCommand(this.PageModel.Code));

            if (result.IsSuccess)
            {
                var returnUrl = await this._authenticationService.SignInUserFromPartialStateAsync(result.Value.UserId);

                if (string.IsNullOrEmpty(returnUrl))
                {
                    return this.RedirectToPage(PageLocations.AppDashboard);
                }

                return this.LocalRedirect(returnUrl);
            }

            this.AddPageNotification("Authentication failed, please try again.", PageNotification.Error);
            this.PrgState = PrgState.Failed;
            return this.RedirectToPage();
        }

        public async Task<IActionResult> OnPostEmailMfaAsync()
        {
            var result =
                await this._mediator.Send(new EmailMfaRequestedCommand());

            if (result.IsSuccess)
            {
                return this.RedirectToPage(PageLocations.AuthEmailMfa);
            }

            this.PrgState = PrgState.Failed;
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

            this.PrgState = PrgState.Failed;
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
                this.RuleFor(x => x.Code)
                    .NotEmpty()
                    .WithMessage("Please enter your verification code.");
            }
        }
    }
}