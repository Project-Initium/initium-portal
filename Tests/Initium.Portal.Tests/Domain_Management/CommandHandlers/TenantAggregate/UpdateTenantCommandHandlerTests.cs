// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Common.Domain.AggregatesModel.TenantAggregate;
using Initium.Portal.Common.Domain.CommandHandlers.TenantAggregate;
using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Domain;
using Initium.Portal.Queries.Management.Contracts;
using Initium.Portal.Queries.Models;
using MaybeMonad;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Domain_Management.CommandHandlers.TenantAggregate
{
    public class UpdateTenantCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var tenantQueries = new Mock<ITenantQueryService>();
            tenantQueries.Setup(x =>
                    x.CheckForPresenceOfTenantByIdentifier(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var tenantRepository = new Mock<ITenantRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            tenantRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var tenant = new Mock<ITenant>();
            tenant.Setup(x => x.Identifier).Returns("identifier");
            tenantRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(tenant.Object));

            var logger = new Mock<ILogger<UpdateTenantCommandHandler>>();

            var handler = new UpdateTenantCommandHandler(tenantRepository.Object, logger.Object, tenantQueries.Object);
            var cmd = new UpdateTenantCommand(
                TestVariables.TenantId,
                "identifier",
                "name",
                new List<SystemFeatures>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var tenantQueries = new Mock<ITenantQueryService>();
            tenantQueries.Setup(x =>
                    x.CheckForPresenceOfTenantByIdentifier(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var tenantRepository = new Mock<ITenantRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            tenantRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            var tenant = new Mock<ITenant>();
            tenant.Setup(x => x.Identifier).Returns("identifier");
            tenantRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(tenant.Object));
            var logger = new Mock<ILogger<UpdateTenantCommandHandler>>();

            var handler = new UpdateTenantCommandHandler(tenantRepository.Object, logger.Object, tenantQueries.Object);
            var cmd = new UpdateTenantCommand(
                TestVariables.TenantId,
                "identifier",
                "name",
                new List<SystemFeatures>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenTenantDoesNotExist_ExpectFailedResult()
        {
            var tenantQueries = new Mock<ITenantQueryService>();
            tenantQueries.Setup(x =>
                    x.CheckForPresenceOfTenantByIdentifier(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var tenantRepository = new Mock<ITenantRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            tenantRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            tenantRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe<ITenant>.Nothing);
            var logger = new Mock<ILogger<UpdateTenantCommandHandler>>();

            var handler = new UpdateTenantCommandHandler(tenantRepository.Object, logger.Object, tenantQueries.Object);
            var cmd = new UpdateTenantCommand(
                TestVariables.TenantId,
                "identifier",
                "name",
                new List<SystemFeatures>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.TenantNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenTenantDoesExistAndIdentifierHasChangeAndIsInUse_ExpectFailedResult()
        {
            var tenantQueries = new Mock<ITenantQueryService>();
            tenantQueries.Setup(x =>
                    x.CheckForPresenceOfTenantByIdentifier(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new StatusCheckModel(true));
            var tenantRepository = new Mock<ITenantRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            tenantRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var tenant = new Mock<ITenant>();
            tenant.Setup(x => x.Identifier).Returns("identifier");
            tenantRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(tenant.Object));
            var logger = new Mock<ILogger<UpdateTenantCommandHandler>>();

            var handler = new UpdateTenantCommandHandler(tenantRepository.Object, logger.Object, tenantQueries.Object);
            var cmd = new UpdateTenantCommand(
                TestVariables.TenantId,
                "identifier-new",
                "name",
                new List<SystemFeatures>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.TenantAlreadyExists, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenTenantDoesExistAndIdentifierHasChangeButIsNotInUse_ExpectTenantUpdatedAndPresenceCheckCalled()
        {
            var tenantQueries = new Mock<ITenantQueryService>();
            tenantQueries.Setup(x =>
                    x.CheckForPresenceOfTenantByIdentifier(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var tenantRepository = new Mock<ITenantRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            tenantRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var tenant = new Mock<ITenant>();
            tenant.Setup(x => x.Identifier).Returns("identifier");
            tenantRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(tenant.Object));
            var logger = new Mock<ILogger<UpdateTenantCommandHandler>>();

            var handler = new UpdateTenantCommandHandler(tenantRepository.Object, logger.Object, tenantQueries.Object);
            var cmd = new UpdateTenantCommand(
                TestVariables.TenantId,
                "identifier-new",
                "name",
                new List<SystemFeatures>());

            await handler.Handle(cmd, CancellationToken.None);

            tenant.Verify(x => x.UpdateDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            tenantRepository.Verify(x => x.Update(It.IsAny<ITenant>()), Times.Once);
            tenantQueries.Verify(x => x.CheckForPresenceOfTenantByIdentifier(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_GivenTenantDoesExistAndIdentifierHasNotChange_ExpectTenantUpdatedAndPresenceCheckNotCalled()
        {
            var tenantQueries = new Mock<ITenantQueryService>();
            tenantQueries.Setup(x =>
                    x.CheckForPresenceOfTenantByIdentifier(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var tenantRepository = new Mock<ITenantRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            tenantRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            var tenant = new Mock<ITenant>();
            tenant.Setup(x => x.Identifier).Returns("identifier");
            tenantRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(tenant.Object));
            var logger = new Mock<ILogger<UpdateTenantCommandHandler>>();

            var handler = new UpdateTenantCommandHandler(tenantRepository.Object, logger.Object, tenantQueries.Object);
            var cmd = new UpdateTenantCommand(
                TestVariables.TenantId,
                "identifier",
                "name",
                new List<SystemFeatures>());

            await handler.Handle(cmd, CancellationToken.None);

            tenant.Verify(x => x.UpdateDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            tenantRepository.Verify(x => x.Update(It.IsAny<ITenant>()), Times.Once);
            tenantQueries.Verify(x => x.CheckForPresenceOfTenantByIdentifier(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}