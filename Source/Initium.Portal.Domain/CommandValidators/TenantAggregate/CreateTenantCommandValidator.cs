// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using FluentValidation;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.TenantAggregate;

namespace Initium.Portal.Domain.CommandValidators.TenantAggregate
{
    public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
    {
        public CreateTenantCommandValidator()
        {
            this.RuleFor(x => x.Id)
                .NotEqual(Guid.Empty).WithErrorCode(ValidationCodes.FieldIsRequired);
            this.RuleFor(x => x.Identifier)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);
            this.RuleFor(x => x.Name)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);
            this.RuleFor(x => x.ConnectionString)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}