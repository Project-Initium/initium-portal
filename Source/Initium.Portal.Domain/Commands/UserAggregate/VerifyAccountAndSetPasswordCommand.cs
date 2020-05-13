// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Domain;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.Commands.UserAggregate
{
    public class VerifyAccountAndSetPasswordCommand : IRequest<ResultWithError<ErrorData>>
    {
        public VerifyAccountAndSetPasswordCommand(string token, string newPassword)
        {
            this.Token = token;
            this.NewPassword = newPassword;
        }

        public string Token { get; }

        public string NewPassword { get; }
    }
}