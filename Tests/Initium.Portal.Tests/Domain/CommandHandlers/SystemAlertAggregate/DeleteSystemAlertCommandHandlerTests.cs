// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.SystemAlertAggregate;
using Initium.Portal.Domain.CommandHandlers.SystemAlertAggregate;
using Initium.Portal.Domain.Commands.SystemAlertAggregate;
using MaybeMonad;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.SystemAlertAggregate
{
    public class DeleteSystemAlertCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSystemAlertDoesNotExist_ExpectFailedResult()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            var systemAlertRepository = new Mock<ISystemAlertRepository>();
            systemAlertRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            systemAlertRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe<ISystemAlert>.Nothing);

            var commandHandler =
                new DeleteSystemAlertCommandHandler(systemAlertRepository.Object, Mock.Of<ILogger<DeleteSystemAlertCommandHandler>>());

            var command = new DeleteSystemAlertCommand(TestVariables.SystemAlertId);

            var result = await commandHandler.Handle(command, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SystemAlertNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenCommandIsValid_ExpectDeleteToBeCalled()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            var systemAlertRepository = new Mock<ISystemAlertRepository>();
            systemAlertRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            systemAlertRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(new Mock<ISystemAlert>().Object));

            var commandHandler =
                new DeleteSystemAlertCommandHandler(systemAlertRepository.Object, Mock.Of<ILogger<DeleteSystemAlertCommandHandler>>());

            var command = new DeleteSystemAlertCommand(TestVariables.SystemAlertId);

            await commandHandler.Handle(command, CancellationToken.None);
            systemAlertRepository.Verify(x => x.Delete(It.IsAny<ISystemAlert>()), Times.Once);
        }

        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            var systemAlertRepository = new Mock<ISystemAlertRepository>();
            systemAlertRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            systemAlertRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(new Mock<ISystemAlert>().Object));

            var commandHandler =
                new DeleteSystemAlertCommandHandler(systemAlertRepository.Object, Mock.Of<ILogger<DeleteSystemAlertCommandHandler>>());

            var command = new DeleteSystemAlertCommand(TestVariables.SystemAlertId);

            var result = await commandHandler.Handle(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessResult()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            var systemAlertRepository = new Mock<ISystemAlertRepository>();
            systemAlertRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            systemAlertRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(new Mock<ISystemAlert>().Object));

            var commandHandler =
                new DeleteSystemAlertCommandHandler(systemAlertRepository.Object, Mock.Of<ILogger<DeleteSystemAlertCommandHandler>>());

            var command = new DeleteSystemAlertCommand(TestVariables.SystemAlertId);

            var result = await commandHandler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }
    }
}