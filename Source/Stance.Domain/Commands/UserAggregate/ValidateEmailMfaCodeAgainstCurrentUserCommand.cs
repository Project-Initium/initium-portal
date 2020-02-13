// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MediatR;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.CommandResults.UserAggregate;

namespace Stance.Domain.Commands.UserAggregate
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