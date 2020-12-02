// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.SystemAlertAggregate;
using Initium.Portal.Domain.CommandHandlers.SystemAlertAggregate;
using Initium.Portal.Domain.Commands.SystemAlertAggregate;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.SystemAlertAggregate
{
    public class CreateNewSystemAlertCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSystemAlertIsNotSaved_ExpectFailedResult()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            var systemAlertRepository = new Mock<ISystemAlertRepository>();
            systemAlertRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            systemAlertRepository.Setup(x => x.Add(It.IsAny<ISystemAlert>()))
                .Returns((ISystemAlert systemAlert) => systemAlert);

            var commandHandler =
                new CreateNewSystemAlertCommandHandler(systemAlertRepository.Object, Mock.Of<ILogger<CreateNewSystemAlertCommandHandler>>());

            var command = new CreateNewSystemAlertCommand(
                "name",
                "message",
                SystemAlertType.Critical,
                TestVariables.Now.AddDays(-1),
                TestVariables.Now.AddDays(1));

            var result = await commandHandler.Handle(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSystemAlertIsSaved_ExpectSuccessResult()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            var systemAlertRepository = new Mock<ISystemAlertRepository>();
            systemAlertRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            systemAlertRepository.Setup(x => x.Add(It.IsAny<ISystemAlert>()))
                .Returns((ISystemAlert systemAlert) => systemAlert);

            var commandHandler =
                new CreateNewSystemAlertCommandHandler(systemAlertRepository.Object, Mock.Of<ILogger<CreateNewSystemAlertCommandHandler>>());

            var command = new CreateNewSystemAlertCommand(
                "name",
                "message",
                SystemAlertType.Critical,
                TestVariables.Now.AddDays(-1),
                TestVariables.Now.AddDays(1));

            var result = await commandHandler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenCommandIsValid_ExpectSystemAlertToBeAdded()
        {
            ISystemAlert systemAlert = null;
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            var systemAlertRepository = new Mock<ISystemAlertRepository>();
            systemAlertRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            systemAlertRepository.Setup(x => x.Add(It.IsAny<ISystemAlert>()))
                .Returns((ISystemAlert n) =>
                {
                    systemAlert = n;
                    return n;
                });

            var commandHandler =
                new CreateNewSystemAlertCommandHandler(systemAlertRepository.Object, Mock.Of<ILogger<CreateNewSystemAlertCommandHandler>>());

            var command = new CreateNewSystemAlertCommand(
                "name",
                "message",
                SystemAlertType.Critical,
                TestVariables.Now.AddDays(-1),
                TestVariables.Now.AddDays(1));

            var result = await commandHandler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(systemAlert);
            Assert.Equal(systemAlert.Id, result.Value.SystemAlertId);
            systemAlertRepository.Verify(x => x.Add(It.IsAny<ISystemAlert>()), Times.Once);
        }
    }
}