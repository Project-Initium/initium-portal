// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Initium.Portal.Web.Controllers.OData.Role
{
    public class RoleFilter
    {
        public bool HasResources { get; set; }

        public bool HasNoResources { get; set; }

        public bool HasUsers { get; set; }

        public bool HasNoUsers { get; set; }
    }
}