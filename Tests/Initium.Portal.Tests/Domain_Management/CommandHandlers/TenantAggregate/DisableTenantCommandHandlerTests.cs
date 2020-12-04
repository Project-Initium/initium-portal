// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Common.Domain.AggregatesModel.TenantAggregate;
using Initium.Portal.Common.Domain.CommandHandlers.TenantAggregate;
using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Domain;
using MaybeMonad;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using Xunit;

namespace Initium.Portal.Tests.Domain_Management.CommandHandlers.TenantAggregate
{
    public class DisableTenantCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var tenantRepository = new Mock<ITenantRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            tenantRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var tenant = new Mock<ITenant>();
            tenant.Setup(x => x.Identifier).Returns("identifier");
            tenantRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From<ITenant>(tenant.Object));

            var logger = new Mock<ILogger<DisableTenantCommandHandler>>();

            var clock = new Mock<IClock>();

            var handler = new DisableTenantCommandHandler(tenantRepository.Object, logger.Object, clock.Object);
            var cmd = new DisableTenantCommand(TestVariables.TenantId);

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var tenantRepository = new Mock<ITenantRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            tenantRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            var tenant = new Mock<ITenant>();

            tenantRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From<ITenant>(tenant.Object));
            var logger = new Mock<ILogger<DisableTenantCommandHandler>>();

            var clock = new Mock<IClock>();

            var handler = new DisableTenantCommandHandler(tenantRepository.Object, logger.Object, clock.Object);
            var cmd = new DisableTenantCommand(TestVariables.TenantId);

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenTenantDoesNotExist_ExpectFailedResult()
        {
            var tenantRepository = new Mock<ITenantRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            tenantRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            tenantRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe<ITenant>.Nothing);
            var logger = new Mock<ILogger<DisableTenantCommandHandler>>();

            var clock = new Mock<IClock>();

            var handler = new DisableTenantCommandHandler(tenantRepository.Object, logger.Object, clock.Object);
            var cmd = new DisableTenantCommand(TestVariables.TenantId);

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.TenantNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenTenantExists_ExpectTenantEnabledAndUpdated()
        {
            var tenantRepository = new Mock<ITenantRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            tenantRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            var tenant = new Mock<ITenant>();

            tenantRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(tenant.Object));
            var logger = new Mock<ILogger<DisableTenantCommandHandler>>();

            var clock = new Mock<IClock>();

            var handler = new DisableTenantCommandHandler(tenantRepository.Object, logger.Object, clock.Object);
            var cmd = new DisableTenantCommand(TestVariables.TenantId);

            await handler.Handle(cmd, CancellationToken.None);

            tenant.Verify(x => x.Disable(It.IsAny<DateTime>()), Times.Once);
            tenantRepository.Verify(x => x.Update(It.IsAny<ITenant>()), Times.Once);
        }
    }
}