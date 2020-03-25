// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Moq;
using Stance.Core.Contracts.Domain;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.RoleAggregate;
using Stance.Domain.CommandHandlers.RoleAggregate;
using Stance.Domain.Commands.RoleAggregate;
using Stance.Queries.Contracts.Static;
using Stance.Queries.Static.Models;
using Xunit;

namespace Stance.Tests.Domain.CommandHandlers.RoleAggregate
{
    public class UpdateRoleCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenRoleDoesExistAndNameHasChangedAndIsUnique_ExpectSuccessfulResultAndRoleUpdated()
        {
            var role = new Mock<IRole>();
            role.Setup(x => x.Name).Returns(new string('*', 5));
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.CheckForPresenceOfRoleByName(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            roleRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(role.Object));

            var handler = new UpdateRoleCommandHandler(roleRepository.Object, roleQueries.Object);
            var cmd = new UpdateRoleCommand(Guid.Empty, new string('*', 6), new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
            roleRepository.Verify(x => x.Update(It.IsAny<IRole>()), Times.Once());
        }

        [Fact]
        public async Task Handle_GivenRoleDoesExistAndNameHasChangedButIsNotUnique_ExpectFailedResult()
        {
            var role = new Mock<IRole>();
            role.Setup(x => x.Name).Returns(new string('*', 5));
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.CheckForPresenceOfRoleByName(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(true));
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            roleRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(role.Object));

            var handler = new UpdateRoleCommandHandler(roleRepository.Object, roleQueries.Object);
            var cmd = new UpdateRoleCommand(Guid.Empty, new string('*', 6), new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.RoleAlreadyExists, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenRoleDoesExistAndNameHasNotChanged_ExpectSuccessfulResultAndRoleUpdated()
        {
            var role = new Mock<IRole>();
            role.Setup(x => x.Name).Returns(new string('*', 5));
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.CheckForPresenceOfRoleByName(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            roleRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(role.Object));

            var handler = new UpdateRoleCommandHandler(roleRepository.Object, roleQueries.Object);
            var cmd = new UpdateRoleCommand(Guid.Empty, new string('*', 5), new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
            roleRepository.Verify(x => x.Update(It.IsAny<IRole>()), Times.Once());
        }

        [Fact]
        public async Task Handle_GivenRoleDoesNotExist_ExpectFailedResult()
        {
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.CheckForPresenceOfRoleByName(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            roleRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe<IRole>.Nothing);

            var handler = new UpdateRoleCommandHandler(roleRepository.Object, roleQueries.Object);
            var cmd = new UpdateRoleCommand(Guid.Empty, string.Empty, new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.RoleNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var role = new Mock<IRole>();
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.CheckForPresenceOfRoleByName(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            roleRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(role.Object));

            var handler = new UpdateRoleCommandHandler(roleRepository.Object, roleQueries.Object);
            var cmd = new UpdateRoleCommand(Guid.Empty, string.Empty, new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var role = new Mock<IRole>();
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.CheckForPresenceOfRoleByName(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            roleRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(role.Object));

            var handler = new UpdateRoleCommandHandler(roleRepository.Object, roleQueries.Object);
            var cmd = new UpdateRoleCommand(Guid.Empty, string.Empty, new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }
    }
}