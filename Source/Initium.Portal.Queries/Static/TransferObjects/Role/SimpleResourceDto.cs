// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Initium.Portal.Queries.Static.TransferObjects.Role
{
    public class SimpleResourceDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid ParentResourceId { get; set; }
    }
}