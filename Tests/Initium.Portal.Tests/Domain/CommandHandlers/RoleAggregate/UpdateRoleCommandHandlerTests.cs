// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.RoleAggregate;
using Initium.Portal.Domain.CommandHandlers.RoleAggregate;
using Initium.Portal.Domain.Commands.RoleAggregate;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Models;
using MaybeMonad;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.RoleAggregate
{
    public class UpdateRoleCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenRoleDoesExistAndNameHasChangedAndIsUnique_ExpectSuccessfulResultAndRoleUpdated()
        {
            var role = new Mock<IRole>();
            role.Setup(x => x.Name).Returns(string.Empty);
            var roleQueries = new Mock<IRoleQueryService>();
            roleQueries.Setup(x => x.CheckForPresenceOfRoleByName(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            roleRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(role.Object));

            var logger = new Mock<ILogger<UpdateRoleCommandHandler>>();

            var handler = new UpdateRoleCommandHandler(roleRepository.Object, roleQueries.Object, logger.Object);
            var cmd = new UpdateRoleCommand(TestVariables.RoleId, "name", new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
            roleRepository.Verify(x => x.Update(It.IsAny<IRole>()), Times.Once());
        }

        [Fact]
        public async Task Handle_GivenRoleDoesExistAndNameHasChangedButIsNotUnique_ExpectFailedResult()
        {
            var role = new Mock<IRole>();
            role.Setup(x => x.Name).Returns(string.Empty);
            var roleQueries = new Mock<IRoleQueryService>();
            roleQueries.Setup(x => x.CheckForPresenceOfRoleByName(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(true));
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            roleRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(role.Object));

            var logger = new Mock<ILogger<UpdateRoleCommandHandler>>();

            var handler = new UpdateRoleCommandHandler(roleRepository.Object, roleQueries.Object, logger.Object);
            var cmd = new UpdateRoleCommand(TestVariables.RoleId, "name", new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.RoleAlreadyExists, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenRoleDoesExistAndNameHasNotChanged_ExpectSuccessfulResultAndRoleUpdated()
        {
            var role = new Mock<IRole>();
            role.Setup(x => x.Name).Returns("name");
            var roleQueries = new Mock<IRoleQueryService>();
            roleQueries.Setup(x => x.CheckForPresenceOfRoleByName(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            roleRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(role.Object));

            var logger = new Mock<ILogger<UpdateRoleCommandHandler>>();

            var handler = new UpdateRoleCommandHandler(roleRepository.Object, roleQueries.Object, logger.Object);
            var cmd = new UpdateRoleCommand(TestVariables.RoleId, "name", new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
            roleRepository.Verify(x => x.Update(It.IsAny<IRole>()), Times.Once());
        }

        [Fact]
        public async Task Handle_GivenRoleDoesNotExist_ExpectFailedResult()
        {
            var roleQueries = new Mock<IRoleQueryService>();
            roleQueries.Setup(x => x.CheckForPresenceOfRoleByName(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            roleRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe<IRole>.Nothing);

            var logger = new Mock<ILogger<UpdateRoleCommandHandler>>();

            var handler = new UpdateRoleCommandHandler(roleRepository.Object, roleQueries.Object, logger.Object);
            var cmd = new UpdateRoleCommand(TestVariables.RoleId, "name", new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.RoleNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var role = new Mock<IRole>();
            var roleQueries = new Mock<IRoleQueryService>();
            roleQueries.Setup(x => x.CheckForPresenceOfRoleByName(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            roleRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(role.Object));

            var logger = new Mock<ILogger<UpdateRoleCommandHandler>>();

            var handler = new UpdateRoleCommandHandler(roleRepository.Object, roleQueries.Object, logger.Object);
            var cmd = new UpdateRoleCommand(TestVariables.RoleId, "name", new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var role = new Mock<IRole>();
            var roleQueries = new Mock<IRoleQueryService>();
            roleQueries.Setup(x => x.CheckForPresenceOfRoleByName(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            roleRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(role.Object));

            var logger = new Mock<ILogger<UpdateRoleCommandHandler>>();

            var handler = new UpdateRoleCommandHandler(roleRepository.Object, roleQueries.Object, logger.Object);
            var cmd = new UpdateRoleCommand(TestVariables.RoleId, "name", new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }
    }
}