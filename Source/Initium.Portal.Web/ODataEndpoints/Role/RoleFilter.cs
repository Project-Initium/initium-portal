// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Web.Infrastructure.ODataEndpoints;

namespace Initium.Portal.Web.ODataEndpoints.Role
{
    public class RoleFilter : IODataFilter
    {
        public bool HasResources { get; set; }

        public bool HasNoResources { get; set; }

        public bool HasUsers { get; set; }

        public bool HasNoUsers { get; set; }
    }
}