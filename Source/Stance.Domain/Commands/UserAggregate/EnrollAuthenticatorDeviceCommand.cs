// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib;
using MediatR;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.CommandResults.UserAggregate;

namespace Stance.Domain.Commands.UserAggregate
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