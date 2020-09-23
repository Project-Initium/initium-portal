// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Initium.Portal.Queries.Models.Tenant
{
    public class TenantMetadata
    {
        public TenantMetadata(Guid id, string identifier, string name, string connectionString, DateTime? whenDisabled)
        {
            this.Id = id;
            this.Identifier = identifier;
            this.Name = name;
            this.ConnectionString = connectionString;
            this.WhenDisabled = whenDisabled;
        }

        public Guid Id { get; }

        public string Identifier { get; }

        public string Name { get; }

        public string ConnectionString { get; }

        public DateTime? WhenDisabled { get; }
    }
}