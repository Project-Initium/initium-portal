// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Initium.Portal.Queries.Static.Models.User
{
    public class DeviceInfo
    {
        public DeviceInfo(Guid id, string name, DateTime whenEnrolled, DateTime? whenLastUsed)
        {
            this.Id = id;
            this.Name = name;
            this.WhenEnrolled = whenEnrolled;
            this.WhenLastUsed = whenLastUsed;
        }

        public Guid Id { get; }

        public string Name { get; }

        public DateTime WhenEnrolled { get; }

        public DateTime? WhenLastUsed { get; }
    }
}