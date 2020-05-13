// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Domain;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.Commands.UserAggregate
{
    public sealed class RevokeAuthenticatorAppCommand : IRequest<ResultWithError<ErrorData>>
    {
        public RevokeAuthenticatorAppCommand(string password)
        {
            this.Password = password;
        }

        public string Password { get; }
    }
}