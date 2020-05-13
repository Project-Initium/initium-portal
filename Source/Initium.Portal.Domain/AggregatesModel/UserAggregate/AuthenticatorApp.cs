// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Domain;

namespace Initium.Portal.Domain.AggregatesModel.UserAggregate
{
    public sealed class AuthenticatorApp : Entity
    {
        public AuthenticatorApp(Guid id, string key, DateTime whenEnrolled)
        {
            this.Id = id;
            this.Key = key;
            this.WhenEnrolled = whenEnrolled;
        }

        private AuthenticatorApp()
        {
        }

        public string Key { get; private set; }

        public DateTime WhenEnrolled { get; private set; }

        public DateTime? WhenRevoked { get; private set; }

        public void RevokeApp(DateTime whenRevoked)
        {
            this.WhenRevoked = whenRevoked;
        }
    }
}