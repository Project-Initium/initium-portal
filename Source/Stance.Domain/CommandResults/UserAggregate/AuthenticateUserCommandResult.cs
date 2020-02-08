// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Stance.Domain.CommandResults.UserAggregate
{
    public class AuthenticateUserCommandResult
    {
        public AuthenticateUserCommandResult(Guid userId, string emailAddress)
        {
            this.UserId = userId;
            this.EmailAddress = emailAddress;
        }

        public Guid UserId { get; }

        public string EmailAddress { get; }
    }
}