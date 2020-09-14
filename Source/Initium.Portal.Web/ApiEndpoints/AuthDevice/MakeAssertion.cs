// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Initium.Portal.Web.ApiEndpoints.AuthDevice
{
    public class MakeAssertion : BaseAsyncEndpoint<AuthenticatorAssertionRawResponse, AssertionVerificationResult>
    {
        private readonly IMediator _mediator;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        private readonly IAuthenticationService _authenticationService;

        public MakeAssertion(IMediator mediator, ITempDataDictionaryFactory tempDataDictionaryFactory, IAuthenticationService authenticationService)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._tempDataDictionaryFactory = tempDataDictionaryFactory ?? throw new ArgumentNullException(nameof(tempDataDictionaryFactory));
            this._authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        [ValidateAntiForgeryToken]
        [HttpPost("api/auth-device/make-assertion", Name = "MakeAssertionEndpoint")]
        [Authorize(AuthenticationSchemes = "login-partial")]
        public override async Task<ActionResult<AssertionVerificationResult>> HandleAsync(
            AuthenticatorAssertionRawResponse request, CancellationToken cancellationToken = default)
        {
            var tempData = this._tempDataDictionaryFactory.GetTempData(this.HttpContext);
            var jsonOptions = tempData["fido2.assertionOptions"] as string;
            var options = AssertionOptions.FromJson(jsonOptions);

            var result =
                await this._mediator.Send(new ValidateDeviceMfaAgainstCurrentUserCommand(request, options), cancellationToken);

            if (!result.IsSuccess)
            {
                return this.Ok(new AssertionVerificationResult { Status = "error" });
            }

            var url = await this._authenticationService.SignInUserFromPartialStateAsync(result.Value.UserId);

            return this.Ok(new
            {
                url = string.IsNullOrWhiteSpace(url) ? this.Url.Page(PageLocations.AppDashboard) : this.LocalRedirect(url).Url,
                assertionVerificationResult = result.Value.AssertionVerificationResult,
            });
        }
    }
}