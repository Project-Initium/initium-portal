// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Domain;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.Commands.UserAggregate
{
    public class PasswordResetCommand : IRequest<ResultWithError<ErrorData>>
    {
        public PasswordResetCommand(Guid token, string newPassword)
        {
            this.Token = token;
            this.NewPassword = newPassword;
        }

        public Guid Token { get; }

        public string NewPassword { get; }
    }
}