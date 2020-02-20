// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MediatR;
using ResultMonad;
using Stance.Core.Domain;

namespace Stance.Domain.Commands.UserAggregate
{
    public class PasswordResetCommand : IRequest<ResultWithError<ErrorData>>
    {
        public PasswordResetCommand(string token, string newPassword)
        {
            this.Token = token;
            this.NewPassword = newPassword;
        }

        public string Token { get; }

        public string NewPassword { get; }
    }
}