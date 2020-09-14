// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Domain;
using Initium.Portal.Core.Settings;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandHandlers.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NodaTime;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.UserAggregate
{
    public class CreateUserCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var clock = new Mock<IClock>();
            var userQueries = new Mock<IUserQueryService>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            userQueries.Setup(x => x.CheckForPresenceOfUserByEmailAddress(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(true));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();

            var logger = new Mock<ILogger<CreateUserCommandHandler>>();

            var handler = new CreateUserCommandHandler(userRepository.Object, clock.Object, userQueries.Object, securitySettings.Object, logger.Object);
            var cmd = new CreateUserCommand("email-address", "first-name", "last-name", false, true, new List<Guid>());
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var clock = new Mock<IClock>();
            var userQueries = new Mock<IUserQueryService>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            userQueries.Setup(x => x.CheckForPresenceOfUserByEmailAddress(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(false));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var logger = new Mock<ILogger<CreateUserCommandHandler>>();

            var handler = new CreateUserCommandHandler(userRepository.Object, clock.Object, userQueries.Object, securitySettings.Object, logger.Object);
            var cmd = new CreateUserCommand("email-address", "first-name", "last-name", false, true, new List<Guid>());
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenUserInSystem_ExpectFailedResult()
        {
            var clock = new Mock<IClock>();
            var userQueries = new Mock<IUserQueryService>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            userQueries.Setup(x => x.CheckForPresenceOfUserByEmailAddress(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(true));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();

            var logger = new Mock<ILogger<CreateUserCommandHandler>>();

            var handler = new CreateUserCommandHandler(userRepository.Object, clock.Object, userQueries.Object, securitySettings.Object, logger.Object);
            var cmd = new CreateUserCommand("email-address", "first-name", "last-name", false, true, new List<Guid>());
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserAlreadyExists, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenUserNotInSystem_ExpectUserToBeAddedAndIdReturned()
        {
            var userId = Guid.Empty;
            var clock = new Mock<IClock>();
            var userQueries = new Mock<IUserQueryService>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Add(It.IsAny<IUser>())).Callback((IUser user) => { userId = user.Id; });

            userQueries.Setup(x => x.CheckForPresenceOfUserByEmailAddress(It.IsAny<string>()))
                .ReturnsAsync(() => new StatusCheckModel(false));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var logger = new Mock<ILogger<CreateUserCommandHandler>>();

            var handler = new CreateUserCommandHandler(userRepository.Object, clock.Object, userQueries.Object, securitySettings.Object, logger.Object);
            var cmd = new CreateUserCommand("email-address", "first-name", "last-name", false, true, new List<Guid>());
            var result = await handler.Handle(cmd, CancellationToken.None);

            userRepository.Verify(x => x.Add(It.IsAny<IUser>()), Times.Once);
            Assert.Equal(userId, result.Value.UserId);
        }
    }
}