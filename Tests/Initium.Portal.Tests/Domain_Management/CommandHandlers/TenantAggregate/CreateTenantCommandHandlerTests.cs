// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Common.Domain.AggregatesModel.TenantAggregate;
using Initium.Portal.Common.Domain.CommandHandlers.TenantAggregate;
using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.Domain;
using Initium.Portal.Queries.Management.Contracts;
using Initium.Portal.Queries.Models;
using Microsoft.Extensions.Logging;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Domain_Management.CommandHandlers.TenantAggregate
{
    public class CreateTenantCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var tenantRepository = new Mock<ITenantRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => ResultWithError.Fail(Mock.Of<IPersistenceError>()));
            tenantRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var logger = new Mock<ILogger<CreateTenantCommandHandler>>();

            var handler = new CreateTenantCommandHandler(tenantRepository.Object, logger.Object);
            var cmd = new CreateTenantCommand(
                TestVariables.TenantId,
                "identifier",
                "name",
                "connection-string",
                new List<SystemFeatures>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var tenantRepository = new Mock<ITenantRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            tenantRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var logger = new Mock<ILogger<CreateTenantCommandHandler>>();

            var handler = new CreateTenantCommandHandler(tenantRepository.Object, logger.Object);
            var cmd = new CreateTenantCommand(
                TestVariables.TenantId,
                "identifier",
                "name",
                "connection-string",
                new List<SystemFeatures>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenTenantAlreadyExists_ExpectFailedResult()
        {
            var tenantRepository = new Mock<ITenantRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(ResultWithError.Fail<IPersistenceError>(new UniquePersistenceError()));
            tenantRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var logger = new Mock<ILogger<CreateTenantCommandHandler>>();

            var handler = new CreateTenantCommandHandler(tenantRepository.Object, logger.Object);
            var cmd = new CreateTenantCommand(
                TestVariables.TenantId,
                "identifier",
                "name",
                "connection-string",
                new List<SystemFeatures>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.TenantAlreadyExists, result.Error.Code);
        }

        [Fact]
        public async Task Handle_ExpectTenantAdded()
        {
            var tenantRepository = new Mock<ITenantRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            tenantRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var logger = new Mock<ILogger<CreateTenantCommandHandler>>();

            var handler = new CreateTenantCommandHandler(tenantRepository.Object, logger.Object);
            var cmd = new CreateTenantCommand(
                TestVariables.TenantId,
                "identifier",
                "name",
                "connection-string",
                new List<SystemFeatures>());

            await handler.Handle(cmd, CancellationToken.None);

            tenantRepository.Verify(x => x.Add(It.IsAny<ITenant>()), Times.Once);
        }
    }
}