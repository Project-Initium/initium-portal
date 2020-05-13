// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Domain;

namespace Initium.Portal.Domain.AggregatesModel.UserAggregate
{
    public sealed class AuthenticatorDevice : Entity
    {
        public AuthenticatorDevice(Guid id, DateTime whenEnrolled, byte[] publicKey, byte[] credentialId, Guid aaguid, int counter, string name, string credType)
        {
            this.Id = id;
            this.WhenEnrolled = whenEnrolled;
            this.PublicKey = publicKey;
            this.CredentialId = credentialId;
            this.Aaguid = aaguid;
            this.Counter = counter;
            this.Name = name;
            this.CredType = credType;
        }

        private AuthenticatorDevice()
        {
        }

        public DateTime WhenEnrolled { get; private set; }

        public DateTime? WhenLastUsed { get; private set; }

        public DateTime? WhenRevoked { get; private set; }

        public byte[] PublicKey { get; private set; }

        public byte[] CredentialId { get; private set; }

        public Guid Aaguid { get; private set; }

        public int Counter { get; private set; }

        public string Name { get; private set; }

        public string CredType { get; private set; }

        public bool IsRevoked => this.WhenRevoked != null;

        public void RevokeDevice(DateTime whenRevoked)
        {
            this.WhenRevoked = whenRevoked;
        }

        public void UpdateCounter(int counter, DateTime whenUsed)
        {
            this.Counter = counter;
            this.WhenLastUsed = whenUsed;
        }
    }
}