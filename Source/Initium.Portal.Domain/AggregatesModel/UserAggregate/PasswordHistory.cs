// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Domain;

namespace Initium.Portal.Domain.AggregatesModel.UserAggregate
{
    public sealed class PasswordHistory : Entity
    {
        public PasswordHistory(Guid id, string hash, DateTime whenUsed)
        {
            this.Id = id;
            this.Hash = hash;
            this.WhenUsed = whenUsed;
        }

        private PasswordHistory()
        {
        }

        public string Hash { get; private set; }

        public DateTime WhenUsed { get; private set; }
    }
}