// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentValidation;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandValidators.UserAggregate
{
    public class
        InitiateAuthenticatorDeviceEnrollmentCommandValidator : AbstractValidator<
            InitiateAuthenticatorDeviceEnrollmentCommand>
    {
    }
}