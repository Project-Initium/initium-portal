// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib.Objects;
using MediatR;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.CommandResults.UserAggregate;

namespace Stance.Domain.Commands.UserAggregate
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