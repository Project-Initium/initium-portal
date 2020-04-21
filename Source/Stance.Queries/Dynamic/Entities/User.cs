﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Stance.Queries.Dynamic.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string EmailAddress { get; set; }

        public bool IsLockable { get; set; }

        public bool IsLocked { get; set; }

        public DateTime? WhenLastAuthenticated { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime WhenCreated { get; set; }

        public bool IsVerified { get; set; }

        public bool IsAdmin { get; set; }
    }
}