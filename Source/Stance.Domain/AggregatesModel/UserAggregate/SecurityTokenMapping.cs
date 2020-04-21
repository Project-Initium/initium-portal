// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Core.Domain;

namespace Stance.Domain.AggregatesModel.UserAggregate
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

        public enum SecurityTokenPurpose
        {
            PasswordReset,
            AccountConfirmation,
        }

        public string Token => Convert.ToBase64String(this.Id.ToByteArray());

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