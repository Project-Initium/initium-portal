// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandHandlers.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Models;
using MaybeMonad;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.UserAggregate
{
    public class UpdateUserCoreDetailsCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenEmailAddressHasChangedAndInUse_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.EmailAddress).Returns("email-address");
            var userQueries = new Mock<IUserQueryService>();
            userQueries.Setup(x =>
                    x.CheckForPresenceOfUserByEmailAddress(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(true));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(user.Object));

            var logger = new Mock<ILogger<UpdateUserCoreDetailsCommandHandler>>();

            var handler = new UpdateUserCoreDetailsCommandHandler(userRepository.Object, userQueries.Object, logger.Object);
            var cmd = new UpdateUserCoreDetailsCommand(TestVariables.UserId, "new-email-address", "first-name",
                "last-name", false, false, new List<Guid>());
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserAlreadyExists, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenEmailAddressHasChangedAndNotInUse_ExpectUserToBeAdded()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.EmailAddress).Returns("email-address");

            var userQueries = new Mock<IUserQueryService>();
            userQueries.Setup(x =>
                    x.CheckForPresenceOfUserByEmailAddress(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(false));

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(user.Object));

            var logger = new Mock<ILogger<UpdateUserCoreDetailsCommandHandler>>();

            var handler = new UpdateUserCoreDetailsCommandHandler(userRepository.Object, userQueries.Object, logger.Object);
            var cmd = new UpdateUserCoreDetailsCommand(TestVariables.UserId, "email-address", "first-name",
                "last-name", false, false, new List<Guid>());
            await handler.Handle(cmd, CancellationToken.None);

            userRepository.Verify(x => x.Update(It.IsAny<IUser>()), Times.Once);
        }

        [Fact]
        public async Task Handle_GivenEmailAddressHasNotChanged_ExpectUserToBeAdded()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.EmailAddress).Returns("email-address");

            var userQueries = new Mock<IUserQueryService>();

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(user.Object));

            var logger = new Mock<ILogger<UpdateUserCoreDetailsCommandHandler>>();

            var handler = new UpdateUserCoreDetailsCommandHandler(userRepository.Object, userQueries.Object, logger.Object);
            var cmd = new UpdateUserCoreDetailsCommand(TestVariables.UserId, "email-address", "first-name",
                "last-name", false, false, new List<Guid>());
            await handler.Handle(cmd, CancellationToken.None);

            userRepository.Verify(x => x.Update(It.IsAny<IUser>()), Times.Once);
            userQueries.Verify(x => x.CheckForPresenceOfUserByEmailAddress(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_GivenNotUserInSystem_ExpectFailedResult()
        {
            var userQueries = new Mock<IUserQueryService>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe<IUser>.Nothing);

            var logger = new Mock<ILogger<UpdateUserCoreDetailsCommandHandler>>();

            var handler = new UpdateUserCoreDetailsCommandHandler(userRepository.Object, userQueries.Object, logger.Object);
            var cmd = new UpdateUserCoreDetailsCommand(TestVariables.UserId, "email-address", "first-name",
                "last-name", false, false, new List<Guid>());
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.EmailAddress).Returns("email-address");
            var userQueries = new Mock<IUserQueryService>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(user.Object));

            var logger = new Mock<ILogger<UpdateUserCoreDetailsCommandHandler>>();

            var handler = new UpdateUserCoreDetailsCommandHandler(userRepository.Object, userQueries.Object, logger.Object);
            var cmd = new UpdateUserCoreDetailsCommand(TestVariables.UserId, "email-address", "first-name",
                "last-name", false, false, new List<Guid>());
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.EmailAddress).Returns("email-address");
            var userQueries = new Mock<IUserQueryService>();

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(user.Object));

            var logger = new Mock<ILogger<UpdateUserCoreDetailsCommandHandler>>();

            var handler = new UpdateUserCoreDetailsCommandHandler(userRepository.Object, userQueries.Object, logger.Object);
            var cmd = new UpdateUserCoreDetailsCommand(TestVariables.UserId, "email-address", "first-name",
                "last-name", false, false, new List<Guid>());
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }
    }
}