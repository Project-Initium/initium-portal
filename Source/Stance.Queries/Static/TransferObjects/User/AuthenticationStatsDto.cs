// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Stance.Queries.Static.TransferObjects.User
{
    public class AuthenticationStatsDto
    {
        public int TotalNewUsers { get; set; }

        public int TotalActiveUsers { get; set; }

        public int TotalLogins { get; set; }

        public int TotalLockedAccounts { get; set; }
    }
}