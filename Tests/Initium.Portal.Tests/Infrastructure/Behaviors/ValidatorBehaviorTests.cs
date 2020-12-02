// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Initium.Portal.Core.Domain;
using Initium.Portal.Core.Exceptions;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.CommandValidators.UserAggregate;
using Initium.Portal.Infrastructure.Behaviors;
using MediatR;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Infrastructure.Behaviors
{
    public class ValidatorBehaviorTests
    {
        [Fact]
        public async Task Handle_GivenNoValidatorIsFound_ExpectNoException()
        {
            var validatorBehavior = new ValidatorBehavior<ValidateAppMfaCodeAgainstCurrentUserCommand,
                Result<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>>(
                new IValidator<ValidateAppMfaCodeAgainstCurrentUserCommand>[] { });

            var requestHandlerDelegate =
                new Mock<RequestHandlerDelegate<Result<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>
                >>();

            await validatorBehavior.Handle(new ValidateAppMfaCodeAgainstCurrentUserCommand("code"), CancellationToken.None,
                requestHandlerDelegate.Object);
            requestHandlerDelegate.Verify(x => x.Invoke(), Times.Once);
        }

        [Fact]
        public async Task Handle_GivenValidatorIsFoundAndCommandIsValid_ExpectNoException()
        {
            var validatorBehavior = new ValidatorBehavior<ValidateAppMfaCodeAgainstCurrentUserCommand,
                Result<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>>(
                new IValidator<ValidateAppMfaCodeAgainstCurrentUserCommand>[]
                {
                    new ValidateAppMfaCodeAgainstCurrentUserCommandValidator(),
                });

            var requestHandlerDelegate =
                new Mock<RequestHandlerDelegate<Result<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>
                >>();

            await validatorBehavior.Handle(new ValidateAppMfaCodeAgainstCurrentUserCommand("code"), CancellationToken.None,
                requestHandlerDelegate.Object);
            requestHandlerDelegate.Verify(x => x.Invoke(), Times.Once);
        }

        [Fact]
        public async Task Handle_GivenValidatorIsFoundAndCommandIsNotValid_ExpectException()
        {
            var validatorBehavior = new ValidatorBehavior<ValidateAppMfaCodeAgainstCurrentUserCommand,
                Result<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>>(
                new IValidator<ValidateAppMfaCodeAgainstCurrentUserCommand>[]
                {
                    new ValidateAppMfaCodeAgainstCurrentUserCommandValidator(),
                });

            var requestHandlerDelegate =
                new Mock<RequestHandlerDelegate<Result<ValidateAppMfaCodeAgainstCurrentUserCommandResult, ErrorData>
                >>();

            await Assert.ThrowsAsync<CustomException>(() => validatorBehavior.Handle(new ValidateAppMfaCodeAgainstCurrentUserCommand(string.Empty), CancellationToken.None,
                    requestHandlerDelegate.Object));
            requestHandlerDelegate.Verify(x => x.Invoke(), Times.Never);
        }
    }
}