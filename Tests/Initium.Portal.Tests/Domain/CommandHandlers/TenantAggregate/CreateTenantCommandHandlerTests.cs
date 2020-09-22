// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.TenantAggregate;
using Initium.Portal.Domain.CommandHandlers.TenantAggregate;
using Initium.Portal.Domain.Commands.TenantAggregate;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.TenantAggregate
{
    public class CreateTenantCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var tenantQueries = new Mock<ITenantQueryService>();
            tenantQueries.Setup(x => x.CheckForPresenceOfTenantByIdentifier(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(() => new StatusCheckModel(false));
            var tenantRepository = new Mock<ITenantRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            tenantRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var logger = new Mock<ILogger<CreateTenantCommandHandler>>();

            var handler = new CreateTenantCommandHandler(tenantRepository.Object, logger.Object, tenantQueries.Object);
            var cmd = new CreateTenantCommand(TestVariables.TenantId, "identifier", "name", "connection-string");

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var tenantQueries = new Mock<ITenantQueryService>();
            tenantQueries.Setup(x => x.CheckForPresenceOfTenantByIdentifier(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var tenantRepository = new Mock<ITenantRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            tenantRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var logger = new Mock<ILogger<CreateTenantCommandHandler>>();

            var handler = new CreateTenantCommandHandler(tenantRepository.Object, logger.Object, tenantQueries.Object);
            var cmd = new CreateTenantCommand(TestVariables.TenantId, "identifier", "name", "connection-string");

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenTenantAlreadyExists_ExpectFailedResult()
        {
            var tenantQueries = new Mock<ITenantQueryService>();
            tenantQueries.Setup(x => x.CheckForPresenceOfTenantByIdentifier(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new StatusCheckModel(true));
            var tenantRepository = new Mock<ITenantRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            tenantRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var logger = new Mock<ILogger<CreateTenantCommandHandler>>();

            var handler = new CreateTenantCommandHandler(tenantRepository.Object, logger.Object, tenantQueries.Object);
            var cmd = new CreateTenantCommand(TestVariables.TenantId, "identifier", "name", "connection-string");

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.TenantAlreadyExists, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenTenantDoesNotExists_ExpectTenantAdded()
        {
            var tenantQueries = new Mock<ITenantQueryService>();
            tenantQueries.Setup(x => x.CheckForPresenceOfTenantByIdentifier(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var tenantRepository = new Mock<ITenantRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            tenantRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var logger = new Mock<ILogger<CreateTenantCommandHandler>>();

            var handler = new CreateTenantCommandHandler(tenantRepository.Object, logger.Object, tenantQueries.Object);
            var cmd = new CreateTenantCommand(TestVariables.TenantId, "identifier", "name", "connection-string");

            await handler.Handle(cmd, CancellationToken.None);

            tenantRepository.Verify(x => x.Add(It.IsAny<ITenant>()), Times.Once);
        }
    }
}