// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using FluentValidation;
using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandValidators.UserAggregate
{
    public class RevokeAuthenticatorDeviceCommandValidator : AbstractValidator<RevokeAuthenticatorDeviceCommand>
    {
        public RevokeAuthenticatorDeviceCommandValidator()
        {
            this.RuleFor(x => x.DeviceId)
                .NotEqual(Guid.Empty).WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}