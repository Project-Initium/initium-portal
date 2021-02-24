// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Domain;

namespace Initium.Portal.Domain.AggregatesModel.UserAggregate
{
    public sealed class SecurityTokenMapping : Entity
    {
        public SecurityTokenMapping(Guid id, SecurityTokenPurpose purpose, DateTime whenCreated, DateTime whenExpires)
        {
            this.Id = id;
            this.Purpose = purpose;
            this.WhenCreated = whenCreated;
            this.WhenExpires = whenExpires;
        }

        private SecurityTokenMapping()
        {
        }

        public SecurityTokenPurpose Purpose { get; private set; }

        public DateTime WhenCreated { get; private set; }

        public DateTime WhenExpires { get; private set; }

        public DateTime? WhenUsed { get; private set; }

        public void MarkUsed(DateTime usedOn)
        {
            this.WhenUsed = usedOn;
        }
    }
}