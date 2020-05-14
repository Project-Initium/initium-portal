// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.Commands.UserAggregate
{
    public class ValidateEmailMfaCodeAgainstCurrentUserCommand : IRequest<Result<ValidateEmailMfaCodeAgainstCurrentUserCommandResult, ErrorData>>
    {
        public ValidateEmailMfaCodeAgainstCurrentUserCommand(string code)
        {
            this.Code = code;
        }

        public string Code { get; }
    }
}