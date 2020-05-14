// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Initium.Portal.Queries.Static.TransferObjects.User
{
    public class DeviceInfoDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime WhenEnrolled { get; set; }

        public DateTime? WhenLastUsed { get; set; }
    }
}