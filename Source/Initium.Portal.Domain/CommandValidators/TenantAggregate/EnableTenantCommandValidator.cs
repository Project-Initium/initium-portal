// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using FluentValidation;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.TenantAggregate;

namespace Initium.Portal.Domain.CommandValidators.TenantAggregate
{
    public class EnableTenantCommandValidator : AbstractValidator<EnableTenantCommand>
    {
        public EnableTenantCommandValidator()
        {
            this.RuleFor(x => x.TenantId)
                .NotEqual(Guid.Empty).WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}