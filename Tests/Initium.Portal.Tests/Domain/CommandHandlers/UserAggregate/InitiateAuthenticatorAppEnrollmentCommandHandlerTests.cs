// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Domain.CommandHandlers.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.UserAggregate
{
    public class InitiateAuthenticatorAppEnrollmentCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GiveValidArguments_ExpectResultWithKeySet()
        {
            var initiateAuthenticatorAppEnrollmentCommandHandler =
                new InitiateAuthenticatorAppEnrollmentCommandHandler();
            var result =
                await initiateAuthenticatorAppEnrollmentCommandHandler.Handle(
                    new InitiateAuthenticatorAppEnrollmentCommand(), CancellationToken.None);
            Assert.True(result.IsSuccess);
            Assert.False(string.IsNullOrWhiteSpace(result.Value.SharedKey));
        }
    }
}