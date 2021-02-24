// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Caching;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Domain.EventHandlers.IntegrationEventHandlers;
using Initium.Portal.Domain.EventHandlers.Models;
using Initium.Portal.Domain.Events.IntegrationEvents;
using MaybeMonad;
using Microsoft.Extensions.Logging;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Domain.EventHandlers.IntegrationEventHandlers
{
    public class ProcessPasswordResetTokenTests
    {
        [Fact]
        public async Task Handle_GivenValidNotification_ExpectInformationLogged()
        {
            var logger = new Mock<ILogger<ProcessPasswordResetToken>>();
            var tenantInfo = new FeatureBasedTenantInfo
            {
                Name = "identifier",
            };
            var dataSerializer = new Mock<IDataSerializer>();
            dataSerializer.Setup(x => x.SerializeToBase64(It.IsAny<SecurityToken>()))
                .Returns(Result.Ok<string>("token"));

            var handler = new ProcessPasswordResetToken(logger.Object, tenantInfo, dataSerializer.Object);
            await handler.Handle(
                new PasswordResetTokenGeneratedIntegrationEvent(
                "email-address",
                "first-name",
                "last-name",
                TestVariables.SecurityTokenMappingId,
                TestVariables.Now), CancellationToken.None);

            logger.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == "Password Reset Token for Tenant identifier with email email-address is token"),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}