// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Authentication;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandHandlers.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using MaybeMonad;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using OtpNet;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.UserAggregate
{
    public class ValidateEmailMfaCodeAgainstCurrentUserCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var securityStamp = Guid.NewGuid();
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            user.Setup(x => x.SecurityStamp).Returns(securityStamp);
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From<IUser>(user.Object));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new ValidateEmailMfaCodeAgainstCurrentUserCommandHandler(userRepository.Object, currentAuthenticatedUserProvider.Object, Mock.Of<IClock>(), Mock.Of<ILogger<ValidateEmailMfaCodeAgainstCurrentUserCommandHandler>>());

            var totp = new Totp(securityStamp.ToByteArray());

            var code = totp.ComputeTotp();

            var cmd = new ValidateEmailMfaCodeAgainstCurrentUserCommand(code);
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
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From<IUser>(user.Object));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new ValidateEmailMfaCodeAgainstCurrentUserCommandHandler(userRepository.Object, currentAuthenticatedUserProvider.Object, Mock.Of<IClock>(), Mock.Of<ILogger<ValidateEmailMfaCodeAgainstCurrentUserCommandHandler>>());

            var totp = new Totp(securityStamp.ToByteArray());

            var code = totp.ComputeTotp();

            var cmd = new ValidateEmailMfaCodeAgainstCurrentUserCommand(code);
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

            var handler = new ValidateEmailMfaCodeAgainstCurrentUserCommandHandler(userRepository.Object, currentAuthenticatedUserProvider.Object, Mock.Of<IClock>(), Mock.Of<ILogger<ValidateEmailMfaCodeAgainstCurrentUserCommandHandler>>());

            var cmd = new ValidateEmailMfaCodeAgainstCurrentUserCommand("code");
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
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new ValidateEmailMfaCodeAgainstCurrentUserCommandHandler(userRepository.Object, currentAuthenticatedUserProvider.Object, Mock.Of<IClock>(), Mock.Of<ILogger<ValidateEmailMfaCodeAgainstCurrentUserCommandHandler>>());

            var cmd = new ValidateEmailMfaCodeAgainstCurrentUserCommand("code");
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
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new ValidateEmailMfaCodeAgainstCurrentUserCommandHandler(userRepository.Object, currentAuthenticatedUserProvider.Object, Mock.Of<IClock>(), Mock.Of<ILogger<ValidateEmailMfaCodeAgainstCurrentUserCommandHandler>>());

            var cmd = new ValidateEmailMfaCodeAgainstCurrentUserCommand("code");
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
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From<IUser>(user.Object));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new ValidateEmailMfaCodeAgainstCurrentUserCommandHandler(userRepository.Object, currentAuthenticatedUserProvider.Object, Mock.Of<IClock>(), Mock.Of<ILogger<ValidateEmailMfaCodeAgainstCurrentUserCommandHandler>>());

            var totp = new Totp(securityStamp.ToByteArray());

            var code = totp.ComputeTotp();

            var cmd = new ValidateEmailMfaCodeAgainstCurrentUserCommand(code);
            var result = await handler.Handle(cmd, CancellationToken.None);

            user.Verify(x => x.ProcessSuccessfulAuthenticationAttempt(It.IsAny<DateTime>()));

            Assert.True(result.IsSuccess);
            Assert.Equal(userId, result.Value.UserId);
        }
    }
}