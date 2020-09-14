// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Initium.Portal.Domain.Commands.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Initium.Portal.Web.ApiEndpoints.AuthDevice
{
    public class InitialAuthDeviceRegistration : BaseAsyncEndpoint<InitialAuthDeviceRegistration.EndpointRequest, CredentialCreateOptions>
    {
        private readonly IMediator _mediator;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

        public InitialAuthDeviceRegistration(IMediator mediator, ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._tempDataDictionaryFactory = tempDataDictionaryFactory ?? throw new ArgumentNullException(nameof(tempDataDictionaryFactory));
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("api/auth-device/initiate-registration", Name = "InitialAuthDeviceRegistrationEndpoint")]
        public override async Task<ActionResult<CredentialCreateOptions>> HandleAsync([FromBody] EndpointRequest request, CancellationToken cancellationToken = default)
        {
            var result = await this._mediator.Send(new InitiateAuthenticatorDeviceEnrollmentCommand(request.AuthenticatorAttachment), cancellationToken);
            if (!result.IsSuccess)
            {
                return this.Ok(new CredentialCreateOptions
                    { Status = "error", ErrorMessage = result.Error.Message });
            }

            var tempData = this._tempDataDictionaryFactory.GetTempData(this.HttpContext);

            tempData["CredentialData"] = result.Value.Options.ToJson();
            return this.Ok(result.Value.Options);
        }

        public class EndpointRequest
        {
            public AuthenticatorAttachment AuthenticatorAttachment { get; set; }
        }
    }
}