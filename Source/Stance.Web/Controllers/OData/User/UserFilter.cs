// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Stance.Web.Controllers.OData.User
{
    public class UserFilter
    {
        public bool Verified { get; set; }

        public bool Unverified { get; set; }

        public bool Locked { get; set; }

        public bool Unlocked { get; set; }

        public bool Admin { get; set; }

        public bool NonAdmin { get; set; }
    }
}