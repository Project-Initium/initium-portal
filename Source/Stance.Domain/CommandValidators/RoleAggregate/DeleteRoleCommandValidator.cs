// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using FluentValidation;
using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.RoleAggregate;

namespace Stance.Domain.CommandValidators.RoleAggregate
{
    public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
    {
        public DeleteRoleCommandValidator()
        {
            this.RuleFor(x => x.RoleId)
                .NotEqual(Guid.Empty).WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}