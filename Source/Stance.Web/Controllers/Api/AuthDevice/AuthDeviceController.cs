// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Fido2NetLib;
using Fido2NetLib.Objects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Infrastructure.Contracts;

namespace Stance.Web.Controllers.Api.AuthDevice
{
    public class AuthDeviceController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMediator _mediator;

        public AuthDeviceController(IMediator mediator, IAuthenticationService authenticationService)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._authenticationService = authenticationService;
        }

        [Authorize]
        [HttpPost("api/auth-device/initiate-registration")]
        public async Task<IActionResult> InitialAuthDeviceRegistration()
        {
            var result = await this._mediator.Send(new InitiateAuthenticatorDeviceEnrollmentCommand());
            if (result.IsSuccess)
            {
                this.TempData["CredentialData"] = result.Value.Options.ToJson();
                return this.Json(result.Value.Options);
            }

            return this.Json(new CredentialCreateOptions
                { Status = "error", ErrorMessage = result.Error.Message });
        }

        [Authorize]
        [HttpPost("api/auth-device/complete-registration")]
        public async Task<IActionResult> CompleteAuthDeviceRegistration(
            [FromBody] CompleteAuthDeviceRegistrationRequest model)
        {
            var jsonOptions = this.TempData["CredentialData"] as string;
            var options = CredentialCreateOptions.FromJson(jsonOptions);

            var result = await this._mediator.Send(new EnrollAuthenticatorDeviceCommand(
                model.Name,
                model.AttestationResponse,
                options));

            return this.Json(result.IsFailure
                ? new Fido2.CredentialMakeResult { Status = "error" }
                : result.Value.CredentialMakeResult);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "login-partial")]
        [Route("api/auth-device/assertion-options")]
        public ActionResult AssertionOptionsPost()
        {
            var options = this.TempData["fido2.assertionOptions"] as string;

            this.TempData["fido2.assertionOptions"] = options;

            return this.Content(options, "application/json");
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "login-partial")]
        [Route("api/auth-device/make-assertion")]
        public async Task<JsonResult> MakeAssertion([FromBody] AuthenticatorAssertionRawResponse clientResponse)
        {
            var jsonOptions = this.TempData["fido2.assertionOptions"] as string;
            var options = AssertionOptions.FromJson(jsonOptions);

            var result =
                await this._mediator.Send(new ValidateDeviceMfaAgainstCurrentUserCommand(clientResponse, options));

            if (!result.IsSuccess)
            {
                return this.Json(new AssertionVerificationResult { Status = "error" });
            }

            var url = await this._authenticationService.SignInUserFromPartialStateAsync(result.Value.UserId);
            return this.Json(new
            {
                url,
                assertionVerificationResult = result.Value.AssertionVerificationResult,
            });
        }
    }
}