// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.RoleAggregate;
using Initium.Portal.Domain.CommandHandlers.RoleAggregate;
using Initium.Portal.Domain.Commands.RoleAggregate;
using MaybeMonad;
using Microsoft.Extensions.Logging;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.RoleAggregate
{
    public class DeleteRoleCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenRoleDoesNotExist_ExpectFailedResult()
        {
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            roleRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe<IRole>.Nothing);

            var handler = new DeleteRoleCommandHandler(roleRepository.Object, Mock.Of<ILogger<DeleteRoleCommandHandler>>());
            var cmd = new DeleteRoleCommand(TestVariables.RoleId);

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.RoleNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenRoleExistsButIsInUse_ExpectFailedResult()
        {
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail<IPersistenceError>(new InUsePersistenceError()));
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            roleRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(Mock.Of<IRole>()));

            var handler = new DeleteRoleCommandHandler(roleRepository.Object, Mock.Of<ILogger<DeleteRoleCommandHandler>>());
            var cmd = new DeleteRoleCommand(TestVariables.RoleId);

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.RoleInUse, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var roleRepository = new Mock<IRoleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => ResultWithError.Fail(Mock.Of<IPersistenceError>()));
            roleRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            roleRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(Mock.Of<IRole>()));

            var handler = new DeleteRoleCommandHandler(roleRepository.Object, Mock.Of<ILogger<DeleteRoleCommandHandler>>());
            var cmd = new DeleteRoleCommand(TestVariables.RoleId);

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
            roleRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(Mock.Of<IRole>()));

            var handler = new DeleteRoleCommandHandler(roleRepository.Object, Mock.Of<ILogger<DeleteRoleCommandHandler>>());
            var cmd = new DeleteRoleCommand(TestVariables.RoleId);

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }
    }
}