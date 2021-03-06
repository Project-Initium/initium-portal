﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using FluentValidation;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.SystemAlertAggregate;

namespace Initium.Portal.Domain.CommandValidators.SystemAlertAggregate
{
    public class DeleteSystemAlertCommandValidator : AbstractValidator<DeleteSystemAlertCommand>
    {
        public DeleteSystemAlertCommandValidator()
        {
            this.RuleFor(x => x.SystemAlertId)
                .Cascade(CascadeMode.Stop)
                .NotEqual(Guid.Empty).WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}