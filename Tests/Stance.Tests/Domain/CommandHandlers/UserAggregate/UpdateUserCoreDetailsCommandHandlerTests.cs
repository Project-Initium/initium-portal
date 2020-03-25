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
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandHandlers.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Stance.Queries.Contracts.Static;
using Stance.Queries.Static.Models;
using Xunit;

namespace Stance.Tests.Domain.CommandHandlers.UserAggregate
{
    public class UpdateUserCoreDetailsCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.EmailAddress).Returns(new string('*', 5));
            var userQueries = new Mock<IUserQueries>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(user.Object));

            var handler = new UpdateUserCoreDetailsCommandHandler(userRepository.Object, userQueries.Object);
            var cmd = new UpdateUserCoreDetailsCommand(Guid.NewGuid(), new string('*', 5), new string('*', 6),
                new string('*', 7), false, false, new List<Guid>());
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.EmailAddress).Returns(new string('*', 5));
            var userQueries = new Mock<IUserQueries>();

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(user.Object));

            var handler = new UpdateUserCoreDetailsCommandHandler(userRepository.Object, userQueries.Object);
            var cmd = new UpdateUserCoreDetailsCommand(Guid.NewGuid(), new string('*', 5), new string('*', 6),
                new string('*', 7), false, false, new List<Guid>());
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenNotUserInSystem_ExpectFailedResult()
        {
            var userQueries = new Mock<IUserQueries>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe<IUser>.Nothing);

            var handler = new UpdateUserCoreDetailsCommandHandler(userRepository.Object, userQueries.Object);
            var cmd = new UpdateUserCoreDetailsCommand(Guid.NewGuid(), new string('*', 5), new string('*', 6),
                new string('*', 7), false, false, new List<Guid>());
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenEmailAddressHasChangedAndInUse_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.EmailAddress).Returns(new string('*', 5));
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x =>
                    x.CheckForPresenceOfUserByEmailAddress(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(true));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(user.Object));

            var handler = new UpdateUserCoreDetailsCommandHandler(userRepository.Object, userQueries.Object);
            var cmd = new UpdateUserCoreDetailsCommand(Guid.NewGuid(), new string('*', 6), new string('*', 6),
                new string('*', 7), false, false, new List<Guid>());
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserAlreadyExists, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenEmailAddressHasChangedAndNotInUse_ExpectUserToBeAdded()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.EmailAddress).Returns(new string('*', 5));

            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x =>
                    x.CheckForPresenceOfUserByEmailAddress(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(false));

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(user.Object));

            var handler = new UpdateUserCoreDetailsCommandHandler(userRepository.Object, userQueries.Object);
            var cmd = new UpdateUserCoreDetailsCommand(Guid.NewGuid(), new string('*', 6), new string('*', 6),
                new string('*', 7), false, false, new List<Guid>());
            await handler.Handle(cmd, CancellationToken.None);

            userRepository.Verify(x => x.Update(It.IsAny<IUser>()), Times.Once);
        }

        [Fact]
        public async Task Handle_GivenEmailAddressHasNotChanged_ExpectUserToBeAdded()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.EmailAddress).Returns(new string('*', 6));

            var userQueries = new Mock<IUserQueries>();

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(user.Object));

            var handler = new UpdateUserCoreDetailsCommandHandler(userRepository.Object, userQueries.Object);
            var cmd = new UpdateUserCoreDetailsCommand(Guid.NewGuid(), new string('*', 6), new string('*', 6),
                new string('*', 7), false, false, new List<Guid>());
            await handler.Handle(cmd, CancellationToken.None);

            userRepository.Verify(x => x.Update(It.IsAny<IUser>()), Times.Once);
            userQueries.Verify(x => x.CheckForPresenceOfUserByEmailAddress(It.IsAny<string>()), Times.Never);
        }
    }
}