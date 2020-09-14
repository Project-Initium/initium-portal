// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Infrastructure;

namespace Initium.Portal.Queries.Entities
{
    public class AuthenticatorApp : ReadEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Key { get; set; }

        public DateTime WhenEnrolled { get; set; }

        public User User { get; set; }
    }
}