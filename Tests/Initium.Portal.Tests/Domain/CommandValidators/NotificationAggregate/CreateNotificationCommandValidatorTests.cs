// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.NotificationAggregate;
using Initium.Portal.Domain.CommandValidators.NotificationAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandValidators.NotificationAggregate
{
    public class CreateNotificationCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new CreateNotificationCommand(
                "subject",
                "message",
                NotificationType.AlphaNotification,
                "serialized-event-data",
                TestVariables.Now,
                new List<Guid>
                {
                    TestVariables.UserId,
                });
            var validator = new CreateNotificationCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenUserIdsIsEmpty_ExpectFail()
        {
            var cmd = new CreateNotificationCommand(
                "subject",
                "message",
                NotificationType.AlphaNotification,
                "serialized-event-data",
                TestVariables.Now,
                new List<Guid>());
            var validator = new CreateNotificationCommandValidator();

            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(result.Errors, x => x.ErrorCode == ValidationCodes.FieldIsRequired && x.PropertyName == "UserIds");
        }
    }
}