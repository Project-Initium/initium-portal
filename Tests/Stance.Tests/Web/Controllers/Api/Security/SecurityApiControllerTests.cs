// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;
using Moq;
using Stance.Web.Controllers.Api.Security;
using Stance.Web.Infrastructure.Formatters;
using Xunit;

namespace Stance.Tests.Web.Controllers.Api.Security
{
    public class SecurityApiControllerTests
    {
        [Fact]
        public void Report_GivenValidRequest_ReportIsLogged()
        {
            var logger = new Mock<ILogger<SecurityApiController>>();

            var securityApiController = new SecurityApiController(logger.Object);

            securityApiController.Report(new CspPost());

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