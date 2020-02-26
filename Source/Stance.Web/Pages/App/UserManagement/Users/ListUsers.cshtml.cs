// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stance.Queries.Contracts;

namespace Stance.Web.Pages.App.UserManagement.Users
{
    [Authorize]
    public class ListUsers : PageModel
    {
        private readonly IUserQueries _userQueries;

        public ListUsers(IUserQueries userQueries)
        {
            this._userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
        }

        public int TotalNewUsers { get; private set; }

        public int TotalActiveUsers { get; private set; }

        public int TotalLogins { get; private set; }

        public int TotalLockedAccounts { get; private set; }

        public async Task OnGetAsync()
        {
            var maybe = await this._userQueries.GetAuthenticationStats();
            if (maybe.HasValue)
            {
                this.TotalNewUsers = maybe.Value.TotalNewUsers;
                this.TotalActiveUsers = maybe.Value.TotalActiveUsers;
                this.TotalLogins = maybe.Value.TotalLogins;
                this.TotalLockedAccounts = maybe.Value.TotalLockedAccounts;
            }
        }
    }
}