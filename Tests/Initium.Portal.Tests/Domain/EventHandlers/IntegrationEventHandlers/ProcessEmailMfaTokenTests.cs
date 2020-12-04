// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Domain.EventHandlers.IntegrationEventHandlers;
using Initium.Portal.Domain.Events.IntegrationEvents;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Domain.EventHandlers.IntegrationEventHandlers
{
    public class ProcessEmailMfaTokenTests
    {
        [Fact]
        public async Task Handle_GivenValidNotification_ExpectInformationLogged()
        {
            var logger = new Mock<ILogger<ProcessEmailMfaToken>>();
            var tenantInfo = new FeatureBasedTenantInfo
            {
                Name = "identifier",
            };

            var handler = new ProcessEmailMfaToken(logger.Object, tenantInfo);
            await handler.Handle(new EmailMfaTokenGeneratedIntegrationEvent("email-address", "first-name", "last-name", "token"), CancellationToken.None);

            logger.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == "MFA Token for Tenant identifier with email email-address is token"),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}