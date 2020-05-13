// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib.Objects;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.Commands.UserAggregate
{
    public class
        InitiateAuthenticatorDeviceEnrollmentCommand : IRequest<
            Result<InitiateAuthenticatorDeviceEnrollmentCommandResult, ErrorData>>
    {
        public InitiateAuthenticatorDeviceEnrollmentCommand(AuthenticatorAttachment authenticatorAttachment)
        {
            this.AuthenticatorAttachment = authenticatorAttachment;
        }

        public AuthenticatorAttachment AuthenticatorAttachment { get; }
    }
}