// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Domain;
using Initium.Portal.Core.Settings;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandHandlers.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.Events;
using MaybeMonad;
using Microsoft.Extensions.Options;
using Moq;
using NodaTime;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.UserAggregate
{
    public class ChangePasswordCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenNoUserAppearsToBeAuthenticate_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            var clock = new Mock<IClock>();

            var handler =
                new ChangePasswordCommandHandler(currentAuthenticatedUserProvider.Object, userRepository.Object, securitySettings.Object, clock.Object);

            var cmd = new ChangePasswordCommand("current-password", "new-password");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("current-password"));
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            user.Setup(x => x.PasswordHistories).Returns(new List<PasswordHistory>
            {
                new PasswordHistory(TestVariables.PasswordHistoryId, BCrypt.Net.BCrypt.HashPassword("new-password"),
                    TestVariables.Now.AddMonths(-1)),
            });

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var systemUser = new Mock<ISystemUser>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());
            var clock = new Mock<IClock>();

            var handler =
                new ChangePasswordCommandHandler(currentAuthenticatedUserProvider.Object, userRepository.Object, securitySettings.Object, clock.Object);

            var cmd = new ChangePasswordCommand("current-password", "new-password");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("current-password"));
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            user.Setup(x => x.PasswordHistories).Returns(new List<PasswordHistory>
            {
                new PasswordHistory(TestVariables.PasswordHistoryId, BCrypt.Net.BCrypt.HashPassword("new-password"),
                    TestVariables.Now.AddMonths(-1)),
            });

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var systemUser = new Mock<ISystemUser>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());
            var clock = new Mock<IClock>();

            var handler =
                new ChangePasswordCommandHandler(currentAuthenticatedUserProvider.Object, userRepository.Object, securitySettings.Object, clock.Object);

            var cmd = new ChangePasswordCommand("current-password", "new-password");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenUserDoesNotExist_ExpectFailedResult()
        {
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe<IUser>.Nothing);

            var systemUser = new Mock<ISystemUser>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            var clock = new Mock<IClock>();

            var handler =
                new ChangePasswordCommandHandler(currentAuthenticatedUserProvider.Object, userRepository.Object, securitySettings.Object, clock.Object);

            var cmd = new ChangePasswordCommand("current-password", "new-password");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenPasswordDoesNotVerify_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var systemUser = new Mock<ISystemUser>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            var clock = new Mock<IClock>();

            var handler =
                new ChangePasswordCommandHandler(currentAuthenticatedUserProvider.Object, userRepository.Object, securitySettings.Object, clock.Object);

            var cmd = new ChangePasswordCommand("current-password", "new-password");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.PasswordNotCorrect, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenPasswordDoesVerify_ExpectPasswordChangedAndDomainEventRaised()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("current-password"));
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            user.Setup(x => x.PasswordHistories).Returns(new List<PasswordHistory>
            {
                new PasswordHistory(TestVariables.PasswordHistoryId, BCrypt.Net.BCrypt.HashPassword("new-password"),
                    TestVariables.Now.AddMonths(-1)),
            });

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var systemUser = new Mock<ISystemUser>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());
            var clock = new Mock<IClock>();

            var handler =
                new ChangePasswordCommandHandler(currentAuthenticatedUserProvider.Object, userRepository.Object, securitySettings.Object, clock.Object);

            var cmd = new ChangePasswordCommand("current-password", "new-password");

            await handler.Handle(cmd, CancellationToken.None);
            user.Verify(x => x.ChangePassword(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            userRepository.Verify(x => x.Update(It.IsAny<IUser>()), Times.Once);
            user.Verify(x => x.AddDomainEvent(It.IsAny<PasswordChangedEvent>()), Times.Once);
        }

        [Fact]
        public async Task Handle_GivenPasswordHasBeenUsedBefore_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("current-password"));
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            user.Setup(x => x.PasswordHistories).Returns(new List<PasswordHistory>
            {
                new PasswordHistory(TestVariables.PasswordHistoryId, BCrypt.Net.BCrypt.HashPassword("new-password"),
                    TestVariables.Now.AddMonths(-1)),
            });

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var systemUser = new Mock<ISystemUser>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings
            {
                HistoricalLimit = 1,
            });
            var clock = new Mock<IClock>();

            var handler =
                new ChangePasswordCommandHandler(currentAuthenticatedUserProvider.Object, userRepository.Object, securitySettings.Object, clock.Object);

            var cmd = new ChangePasswordCommand("current-password", "new-password");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.PasswordInHistory, result.Error.Code);
        }
    }
}