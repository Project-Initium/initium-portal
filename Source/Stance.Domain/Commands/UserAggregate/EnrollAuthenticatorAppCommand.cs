// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MediatR;
using ResultMonad;
using Stance.Core.Domain;

namespace Stance.Domain.Commands.UserAggregate
{
    public class EnrollAuthenticatorAppCommand : IRequest<ResultWithError<ErrorData>>
    {
        public EnrollAuthenticatorAppCommand(string key, string code)
        {
            this.Key = key;
            this.Code = code;
        }

        public string Key { get; }

        public string Code { get; }
    }
}