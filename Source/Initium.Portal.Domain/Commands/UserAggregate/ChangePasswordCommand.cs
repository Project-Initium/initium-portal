// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Domain;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.Commands.UserAggregate
{
    public sealed class ChangePasswordCommand : IRequest<ResultWithError<ErrorData>>
    {
        public ChangePasswordCommand(string currentPassword, string newPassword)
        {
            this.CurrentPassword = currentPassword;
            this.NewPassword = newPassword;
        }

        public string CurrentPassword { get; }

        public string NewPassword { get; }
    }
}