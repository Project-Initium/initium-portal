// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Domain;
using Initium.Portal.Core.Exceptions;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Infrastructure.Behaviors;
using MaybeMonad;
using MediatR;
using Moq;
using ResultMonad;
using Serilog;
using Xunit;

namespace Initium.Portal.Tests.Infrastructure.Behaviors
{
    public class AuditBehaviorTests
    {
        [Fact]
        public async Task Handle_GivenResponseIsResultAndIsNotSuccessful_ExpectErrorLogged()
        {
            var auditLogger = new Mock<ILogger>();
            var authenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            authenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(
                Maybe.From<ISystemUser>(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None)));
            var request = new ValidateAppMfaCodeAgainstCurrentUserCommand("code");

            var auditBehavior =
                new AuditBehavior<ValidateAppMfaCodeAgainstCurrentUserCommand,
                    Result<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>>(
                    auditLogger.Object, authenticatedUserProvider.Object);

            var requestHandlerDelegate =
                new Mock<RequestHandlerDelegate<Result<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>
                >>();
            requestHandlerDelegate.Setup(x => x())
                .ReturnsAsync(Result.Fail<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>(
                    new ErrorData(ErrorCodes.SavingChanges)));

            await auditBehavior.Handle(request, CancellationToken.None, requestHandlerDelegate.Object);

            auditLogger.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<ValidateAppMfaCodeAgainstCurrentUserCommand>(),
                It.Is<string>(x => x == TestVariables.UserId.ToString())));
        }

        [Fact]
        public async Task Handle_GivenResponseIsResultAndIsSuccessful_ExpectInformationLogged()
        {
            var auditLogger = new Mock<ILogger>();
            var authenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            authenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(
                Maybe.From<ISystemUser>(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None)));
            var request = new ValidateAppMfaCodeAgainstCurrentUserCommand("code");

            var auditBehavior =
                new AuditBehavior<ValidateAppMfaCodeAgainstCurrentUserCommand,
                    Result<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>>(
                    auditLogger.Object, authenticatedUserProvider.Object);

            var requestHandlerDelegate =
                new Mock<RequestHandlerDelegate<Result<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>
                >>();
            requestHandlerDelegate.Setup(x => x())
                .ReturnsAsync(Result.Ok<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>(
                    new ValidateAppMfaCodeAgainstCurrentUserCommandResult(TestVariables.UserId)));

            await auditBehavior.Handle(request, CancellationToken.None, requestHandlerDelegate.Object);

            auditLogger.Verify(x => x.Information(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<ValidateAppMfaCodeAgainstCurrentUserCommand>(),
                It.Is<string>(x => x == TestVariables.UserId.ToString())));
        }

        [Fact]
        public async Task Handle_GivenResponseIsResultWithErrorAndIsNotSuccessful_ExpectInformationLogged()
        {
            var auditLogger = new Mock<ILogger>();
            var authenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            authenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(
                Maybe.From<ISystemUser>(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None)));
            var request = new UnlockAccountCommand(TestVariables.UserId);

            var auditBehavior =
                new AuditBehavior<UnlockAccountCommand,
                    ResultWithError<ErrorData>>(
                    auditLogger.Object, authenticatedUserProvider.Object);

            var requestHandlerDelegate =
                new Mock<RequestHandlerDelegate<ResultWithError<ErrorData>>>();
            requestHandlerDelegate.Setup(x => x())
                .ReturnsAsync(ResultWithError.Fail(
                    new ErrorData(ErrorCodes.SavingChanges)));

            await auditBehavior.Handle(request, CancellationToken.None, requestHandlerDelegate.Object);

            auditLogger.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<UnlockAccountCommand>(),
                It.Is<string>(x => x == TestVariables.UserId.ToString())));
        }

        [Fact]
        public async Task Handle_GivenResponseIsResultWithErrorAndIsSuccessful_ExpectInformationLogged()
        {
            var auditLogger = new Mock<ILogger>();
            var authenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            authenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(
                Maybe.From<ISystemUser>(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None)));
            var request = new UnlockAccountCommand(TestVariables.UserId);

            var auditBehavior =
                new AuditBehavior<UnlockAccountCommand,
                    ResultWithError<ErrorData>>(
                    auditLogger.Object, authenticatedUserProvider.Object);

            var requestHandlerDelegate =
                new Mock<RequestHandlerDelegate<ResultWithError<ErrorData>>>();
            requestHandlerDelegate.Setup(x => x())
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());

            await auditBehavior.Handle(request, CancellationToken.None, requestHandlerDelegate.Object);

            auditLogger.Verify(x => x.Information(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<UnlockAccountCommand>(),
                It.Is<string>(x => x == TestVariables.UserId.ToString())));
        }

        [Fact]
        public async Task Handle_GivenResponseIsUnknownType_ExpectExpection()
        {
            var auditLogger = new Mock<ILogger>();
            var authenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();

            var auditBehavior =
                new AuditBehavior<string, string>(auditLogger.Object, authenticatedUserProvider.Object);

            var requestHandlerDelegate = new Mock<RequestHandlerDelegate<string>>();
            requestHandlerDelegate.Setup(x => x())
                .ReturnsAsync(string.Empty);

            await Assert.ThrowsAsync<StanceException>(() => auditBehavior.Handle("some-request", CancellationToken.None, requestHandlerDelegate.Object));
        }
    }
}