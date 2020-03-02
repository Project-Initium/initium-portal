// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Core.Domain;

namespace Stance.Domain.AggregatesModel.UserAggregate
{
    public sealed class UserRole : Entity
    {
        public UserRole(Guid id)
        {
            this.Id = id;
        }

        private UserRole()
        {
        }
    }
}