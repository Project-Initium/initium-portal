// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Moq;
using NodaTime;
using OtpNet;
using Stance.Core;
using Stance.Core.Constants;
using Stance.Core.Contracts;
using Stance.Core.Contracts.Domain;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandHandlers.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandHandlers.UserAggregate
{
    public class ValidateEmailMfaCodeAgainstCurrentUserCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var securityStamp = Guid.NewGuid();
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(Guid.Empty, new string('*', 7), new string('*', 8)));
            user.Setup(x => x.SecurityStamp).Returns(securityStamp);
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From<IUser>(user.Object));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(Guid.NewGuid(), MfaProvider.None) as ISystemUser));

            var clock = new Mock<IClock>();

            var handler = new ValidateEmailMfaCodeAgainstCurrentUserCommandHandler(userRepository.Object, currentAuthenticatedUserProvider.Object, clock.Object);

            var totp = new Totp(securityStamp.ToByteArray());

            var generated = totp.ComputeTotp();

            var cmd = new ValidateEmailMfaCodeAgainstCurrentUserCommand(generated);
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var securityStamp = Guid.NewGuid();
            var user = new Mock<IUser>();
            user.Setup(x => x.SecurityStamp).Returns(securityStamp);
            user.Setup(x => x.Profile).Returns(new Profile(Guid.Empty, new string('*', 7), new string('*', 8)));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From<IUser>(user.Object));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(Guid.NewGuid(), MfaProvider.None) as ISystemUser));

            var clock = new Mock<IClock>();

            var handler = new ValidateEmailMfaCodeAgainstCurrentUserCommandHandler(userRepository.Object, currentAuthenticatedUserProvider.Object, clock.Object);

            var totp = new Totp(securityStamp.ToByteArray());

            var generated = totp.ComputeTotp();

            var cmd = new ValidateEmailMfaCodeAgainstCurrentUserCommand(generated);
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenNoUserAppearsToBeAuthenticate_ExpectFailedResult()
        {
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);

            var clock = new Mock<IClock>();

            var handler = new ValidateEmailMfaCodeAgainstCurrentUserCommandHandler(userRepository.Object, currentAuthenticatedUserProvider.Object, clock.Object);

            var cmd = new ValidateEmailMfaCodeAgainstCurrentUserCommand(new string('*', 4));
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
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
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(Guid.NewGuid(), MfaProvider.None) as ISystemUser));

            var clock = new Mock<IClock>();

            var handler = new ValidateEmailMfaCodeAgainstCurrentUserCommandHandler(userRepository.Object, currentAuthenticatedUserProvider.Object, clock.Object);

            var cmd = new ValidateEmailMfaCodeAgainstCurrentUserCommand(new string('*', 4));
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GiveUserDoesExistButCodeIsNotValid_ExpectFailedResultAndPartialAttemptLogged()
        {
            var user = new Mock<IUser>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From<IUser>(user.Object));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(Guid.NewGuid(), MfaProvider.None) as ISystemUser));

            var clock = new Mock<IClock>();

            var handler = new ValidateEmailMfaCodeAgainstCurrentUserCommandHandler(userRepository.Object, currentAuthenticatedUserProvider.Object, clock.Object);

            var cmd = new ValidateEmailMfaCodeAgainstCurrentUserCommand(new string('*', 4));
            var result = await handler.Handle(cmd, CancellationToken.None);

            user.Verify(x => x.ProcessPartialSuccessfulAuthenticationAttempt(It.IsAny<DateTime>(), AuthenticationHistoryType.EmailMfaFailed));

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.MfaCodeNotValid, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GiveUserDoesExistButCodeIsValid_ExpectSuccessfulResultAndSuccessfulAttemptLogged()
        {
            var securityStamp = Guid.NewGuid();
            var user = new Mock<IUser>();
            var userId = Guid.NewGuid();
            user.Setup(x => x.Id).Returns(userId);
            user.Setup(x => x.EmailAddress).Returns(new string('*', 5));
            user.Setup(x => x.SecurityStamp).Returns(securityStamp);
            user.Setup(x => x.Profile).Returns(new Profile(Guid.Empty, new string('*', 7), new string('*', 8)));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From<IUser>(user.Object));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(Guid.NewGuid(), MfaProvider.None) as ISystemUser));

            var clock = new Mock<IClock>();

            var handler = new ValidateEmailMfaCodeAgainstCurrentUserCommandHandler(userRepository.Object, currentAuthenticatedUserProvider.Object, clock.Object);

            var totp = new Totp(securityStamp.ToByteArray());

            var generated = totp.ComputeTotp();

            var cmd = new ValidateEmailMfaCodeAgainstCurrentUserCommand(generated);
            var result = await handler.Handle(cmd, CancellationToken.None);

            user.Verify(x => x.ProcessSuccessfulAuthenticationAttempt(It.IsAny<DateTime>()));

            Assert.True(result.IsSuccess);
            Assert.Equal(userId, result.Value.UserId);
        }
    }
}