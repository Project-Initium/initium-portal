// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Domain;

namespace Initium.Portal.Domain.AggregatesModel.UserAggregate
{
    public sealed class AuthenticationHistory : Entity
    {
        public AuthenticationHistory(Guid id, DateTime whenHappened, AuthenticationHistoryType authenticationHistoryType)
        {
            this.Id = id;
            this.WhenHappened = whenHappened;
            this.AuthenticationHistoryType = authenticationHistoryType;
        }

        private AuthenticationHistory()
        {
        }

        public DateTime WhenHappened { get; private set; }

        public AuthenticationHistoryType AuthenticationHistoryType { get; private set; }
    }
}