// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Initium.Portal.Queries.Static.TransferObjects.User
{
    public class DetailedUserDto
    {
        public Guid Id { get; set; }

        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsLockable { get; set; }

        public DateTime WhenCreated { get; set; }

        public DateTime? WhenLastAuthenticated { get; set; }

        public DateTime? WhenLocked { get; set; }

        public bool IsAdmin { get; set; }

        public DateTime? WhenDisabled { get; set; }
    }
}