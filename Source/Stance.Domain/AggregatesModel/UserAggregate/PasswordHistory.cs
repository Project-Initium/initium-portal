// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Core.Domain;

namespace Stance.Domain.AggregatesModel.UserAggregate
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