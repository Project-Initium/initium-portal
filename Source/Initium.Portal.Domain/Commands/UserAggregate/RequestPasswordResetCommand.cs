// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Domain;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.Commands.UserAggregate
{
    public sealed class RequestPasswordResetCommand : IRequest<ResultWithError<ErrorData>>
    {
        public RequestPasswordResetCommand(string emailAddress)
        {
            this.EmailAddress = emailAddress;
        }

        public string EmailAddress { get; }
    }
}