// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Stance.Domain.CommandResults.UserAggregate
{
    public sealed class CreateUserCommandResult
    {
        public CreateUserCommandResult(Guid userId)
        {
            this.UserId = userId;
        }

        public Guid UserId { get; }
    }
}