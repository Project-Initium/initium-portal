// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Constants;
using Newtonsoft.Json;

namespace Initium.Portal.Queries.Management.Tenant
{
    public class TenantMetadata
    {
        public TenantMetadata(Guid id, string identifier, string name, DateTime? whenDisabled)
        {
            this.Id = id;
            this.Identifier = identifier;
            this.Name = name;
            this.WhenDisabled = whenDisabled;
            this.HasLoggingInfo = false;
        }

        public Guid Id { get; }

        public string Identifier { get; }

        public string Name { get; }

        public DateTime? WhenDisabled { get; }

        public Guid? LastLoggedInUserId { get; private set; }

        public string LastLoggedInUser { get; private set; }

        public DateTime? WhenLoggedIn { get; private set; }

        public bool HasLoggingInfo { get; private set; }

        public IReadOnlyList<SystemFeatures> SystemFeatures { get; private set; }

        public void SetLoggingInfo(Guid lastLoggedInUserId, string lastLoggedInUser, DateTime whenLoggedIn)
        {
            this.LastLoggedInUser = lastLoggedInUser;
            this.LastLoggedInUserId = lastLoggedInUserId;
            this.WhenLoggedIn = whenLoggedIn;
            this.HasLoggingInfo = true;
        }

        public void SetSystemFeatures(string serializedSystemFeatures)
        {
            if (string.IsNullOrWhiteSpace(serializedSystemFeatures))
            {
                this.SystemFeatures = new List<SystemFeatures>();
                return;
            }

            var data = JsonConvert.DeserializeObject<List<SystemFeatures>>(serializedSystemFeatures);
            this.SystemFeatures = data ?? new List<SystemFeatures>();
        }
    }
}