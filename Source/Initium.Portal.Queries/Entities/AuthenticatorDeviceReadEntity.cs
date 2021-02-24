// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Contracts.Queries;

namespace Initium.Portal.Queries.Entities
{
    public class AuthenticatorDeviceReadEntity : ReadOnlyEntity
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public string Name { get; private set; }

        public DateTime WhenEnrolled { get; private set; }

        public DateTime? WhenLastUsed { get; private set; }

        public byte[] CredentialId { get; private set; }

        public byte[] PublicKey { get; private set; }

        public Guid Aaguid { get; private set; }

        public int Counter { get; private set; }

        public string CredType { get; private set; }

        public UserReadEntity User { get; private set; }
    }
}