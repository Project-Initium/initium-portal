// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using FluentValidation;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.RoleAggregate;

namespace Initium.Portal.Domain.CommandValidators.RoleAggregate
{
    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleCommandValidator()
        {
            this.RuleFor(x => x.Name)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);
            this.RuleFor(x => x.RoleId)
                .NotEqual(Guid.Empty).WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}