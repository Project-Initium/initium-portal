// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    public class CreateRoleCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenRoleAlreadyExists_ExpectFailedResult()
        {
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.CheckForPresenceOfRoleByName(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(true));
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var handler = new CreateRoleCommandHandler(roleRepository.Object, roleQueries.Object);
            var cmd = new CreateRoleCommand(string.Empty, new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.RoleAlreadyExists, result.Error.Code);
            roleRepository.Verify(x => x.Add(It.IsAny<IRole>()), Times.Never);
        }

        [Fact]
        public async Task Handle_GivenRoleDoesNotExists_ExpectSuccessfulResultWithIdSetAndResourcesSetAndRoleAdded()
        {
            var roleId = Guid.Empty;
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.CheckForPresenceOfRoleByName(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            roleRepository.Setup(x => x.Add(It.IsAny<IRole>())).Callback((IRole role) => { roleId = role.Id; });

            var handler = new CreateRoleCommandHandler(roleRepository.Object, roleQueries.Object);
            var cmd = new CreateRoleCommand(string.Empty, new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(roleId, result.Value.RoleId);
            roleRepository.Verify(x => x.Add(It.IsAny<IRole>()), Times.Once);
        }

        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.CheckForPresenceOfRoleByName(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var handler = new CreateRoleCommandHandler(roleRepository.Object, roleQueries.Object);
            var cmd = new CreateRoleCommand(string.Empty, new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.CheckForPresenceOfRoleByName(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(false));
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var handler = new CreateRoleCommandHandler(roleRepository.Object, roleQueries.Object);
            var cmd = new CreateRoleCommand(string.Empty, new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }
    }
}