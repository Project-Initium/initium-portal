// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Stance.Queries.Models.User
{
    public class AuthenticationStatsModel
    {
        public AuthenticationStatsModel(int totalNewUsers, int totalActiveUsers, int totalLogins,
            int totalLockedAccounts)
        {
            this.TotalNewUsers = totalNewUsers;
            this.TotalActiveUsers = totalActiveUsers;
            this.TotalLogins = totalLogins;
            this.TotalLockedAccounts = totalLockedAccounts;
        }

        public int TotalNewUsers { get; }

        public int TotalActiveUsers { get; }

        public int TotalLogins { get; }

        public int TotalLockedAccounts { get; }
    }
}