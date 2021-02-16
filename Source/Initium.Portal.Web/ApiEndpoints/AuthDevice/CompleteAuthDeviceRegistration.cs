// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Fido2NetLib;
using FluentValidation;
using Initium.Portal.Domain.Commands.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Initium.Portal.Web.ApiEndpoints.AuthDevice
{
    public class CompleteAuthDeviceRegistration : BaseAsyncEndpoint<CompleteAuthDeviceRegistration.EndpointRequest,  CompleteAuthDeviceRegistration.EndpointResponse>
    {
        private readonly IMediator _mediator;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

        public CompleteAuthDeviceRegistration(IMediator mediator, ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            this._mediator = mediator;
            this._tempDataDictionaryFactory = tempDataDictionaryFactory;
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("api/auth-device/complete-registration", Name = "CompleteAuthDeviceRegistrationEndpoint")]
        public override async Task<ActionResult<EndpointResponse>> HandleAsync([FromBody] EndpointRequest request, CancellationToken cancellationToken = default)
        {
            var tempData = this._tempDataDictionaryFactory.GetTempData(this.HttpContext);
            var jsonOptions = tempData["CredentialData"] as string;
            var options = CredentialCreateOptions.FromJson(jsonOptions);

            var result = await this._mediator.Send(new EnrollAuthenticatorDeviceCommand(
                request.Name,
                request.AttestationResponse,
                options));

            return this.Ok(result.IsFailure
                ? new EndpointResponse()
                : new EndpointResponse(result.Value.CredentialMakeResult, result.Value.DeviceId, request.Name));
}

        public class EndpointRequest
        {
            public AuthenticatorAttestationRawResponse AttestationResponse { get; set; }

            public string Name { get; set; }
        }

        public class
            EndpointRequestValidator : AbstractValidator<EndpointRequest>
        {
            public EndpointRequestValidator()
            {
                this.RuleFor(x => x.Name)
                    .NotEmpty();
            }
        }

        public class EndpointResponse
        {
            public EndpointResponse()
            {
                this.DeviceId = Guid.Empty;
                this.CredentialMakeResult = new Fido2.CredentialMakeResult { Status = "error" };
            }

            public EndpointResponse(Fido2.CredentialMakeResult result, Guid deviceId, string name)
            {
                this.CredentialMakeResult = result;
                this.DeviceId = deviceId;
                this.Name = name;
            }

            public Fido2.CredentialMakeResult CredentialMakeResult { get; }

            public Guid DeviceId { get; }

            public string Name { get; }
        }
    }
}