// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Web.ApiEndpoints.Security;
using Initium.Portal.Web.Infrastructure.Formatters;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Web.ApiEndpoints.Security
{
    public class ReportTests
    {
        [Fact]
        public void Handle_GivenValidRequest_ReportIsLogged()
        {
            var logger = new Mock<ILogger<Report>>();

            var endpoint = new Report(logger.Object);

            endpoint.Handle(new CspPost());

            logger.Verify(
                l => l.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}