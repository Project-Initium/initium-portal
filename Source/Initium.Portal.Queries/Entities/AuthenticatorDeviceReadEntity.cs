// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Infrastructure;

namespace Initium.Portal.Queries.Entities
{
    public class AuthenticatorDeviceReadEntity : ReadEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Name { get; set; }

        public DateTime WhenEnrolled { get; set; }

        public DateTime? WhenLastUsed { get; set; }

        public byte[] CredentialId { get; set; }

        public byte[] PublicKey { get; set; }

        public Guid Aaguid { get; set; }

        public int Counter { get; set; }

        public string CredType { get; set; }

        public UserReadEntity User { get; set; }
    }
}