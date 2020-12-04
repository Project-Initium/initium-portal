// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Initium.Portal.Core.Authentication;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.PageModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Pages.Auth
{
    [Authorize(AuthenticationSchemes = "login-partial")]
    public class ValidateDeviceMfa : NotificationPageModel
    {
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IMediator _mediator;

        public ValidateDeviceMfa(ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider, IMediator mediator)
        {
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
            this._mediator = mediator;
        }

        public bool HasApp { get; private set; }

        public void OnGet()
        {
            var maybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (maybe.HasValue && maybe.Value is UnauthenticatedUser user)
            {
                this.HasApp = user.SetupMfaProviders.HasFlag(MfaProvider.App);
            }
        }

        public async Task<IActionResult> OnPostEmailMfaAsync()
        {
            var result =
                await this._mediator.Send(new EmailMfaRequestedCommand());

            return result.IsSuccess ? this.RedirectToPage(CorePageLocations.AuthEmailMfa) : this.RedirectToPage();
        }

        public async Task<IActionResult> OnPostAppMfaAsync()
        {
            var result =
                await this._mediator.Send(new AppMfaRequestedCommand());

            return result.IsSuccess ? this.RedirectToPage(CorePageLocations.AuthAppMfa) : this.RedirectToPage();
        }
    }
}