// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stance.Core;
using Stance.Core.Constants;
using Stance.Core.Contracts;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Infrastructure.Constants;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.Auth
{
    [Authorize(AuthenticationSchemes = "login-partial")]
    public class ValidateDeviceMfa : PrgPageModel<object>
    {
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IMediator _mediator;

        public ValidateDeviceMfa(ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider, IMediator mediator)
        {
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
            this._mediator = mediator;
        }

        public bool HasApp { get; set; }

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

            if (result.IsSuccess)
            {
                return this.RedirectToPage(PageLocations.AuthEmailMfa);
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
    }
}