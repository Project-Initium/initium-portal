// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentValidation;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.NotificationAggregate;

namespace Initium.Portal.Domain.CommandValidators.NotificationAggregate
{
    public class CreateNotificationCommandValidator : AbstractValidator<CreateNotificationCommand>
    {
        public CreateNotificationCommandValidator()
        {
            this.RuleFor(x => x.UserIds).NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}