// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Domain;

namespace Initium.Portal.Domain.AggregatesModel.UserAggregate
{
    public sealed class Profile : Entity
    {
        public Profile(Guid id, string firstName, string lastName)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        private Profile()
        {
        }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public void UpdateProfile(string firstName, string lastName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
        }
    }
}