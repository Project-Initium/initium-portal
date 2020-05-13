// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.Commands.UserAggregate
{
    public class EnrollAuthenticatorDeviceCommand : IRequest<Result<EnrollAuthenticatorDeviceCommandResult, ErrorData>>
    {
        public EnrollAuthenticatorDeviceCommand(string name, AuthenticatorAttestationRawResponse authenticatorAttestationRawResponse,
            CredentialCreateOptions credentialCreateOptions)
        {
            this.AuthenticatorAttestationRawResponse = authenticatorAttestationRawResponse;
            this.CredentialCreateOptions = credentialCreateOptions;
            this.Name = name;
        }

        public AuthenticatorAttestationRawResponse AuthenticatorAttestationRawResponse { get; }

        public CredentialCreateOptions CredentialCreateOptions { get; }

        public string Name { get; }
    }
}