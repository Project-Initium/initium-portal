// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.RoleAggregate;
using Initium.Portal.Domain.CommandHandlers.RoleAggregate;
using Initium.Portal.Domain.Commands.RoleAggregate;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Models;
using Microsoft.Extensions.Logging;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.RoleAggregate
{
    public class CreateRoleCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenRoleAlreadyExists_ExpectFailedResult()
        {
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail<IPersistenceError>(new UniquePersistenceError()));
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var handler = new CreateRoleCommandHandler(roleRepository.Object, Mock.Of<ILogger<CreateRoleCommandHandler>>());
            var cmd = new CreateRoleCommand("name", new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.RoleAlreadyExists, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenRoleDoesNotExists_ExpectSuccessfulResultWithIdSetAndResourcesSetAndRoleAdded()
        {
            var roleId = Guid.Empty;
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            roleRepository.Setup(x => x.Add(It.IsAny<IRole>()))
                .Callback((IRole role) => { roleId = role.Id; });

            var handler = new CreateRoleCommandHandler(roleRepository.Object, Mock.Of<ILogger<CreateRoleCommandHandler>>());
            var cmd = new CreateRoleCommand("name", new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(roleId, result.Value.RoleId);
            roleRepository.Verify(x => x.Add(It.IsAny<IRole>()), Times.Once);
        }

        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(Mock.Of<IPersistenceError>()));
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var handler = new CreateRoleCommandHandler(roleRepository.Object, Mock.Of<ILogger<CreateRoleCommandHandler>>());
            var cmd = new CreateRoleCommand("name", new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var handler = new CreateRoleCommandHandler(roleRepository.Object, Mock.Of<ILogger<CreateRoleCommandHandler>>());
            var cmd = new CreateRoleCommand("name", new List<Guid>());

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }
    }
}