// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Stance.Web.Pages.App.UserManagement.Users
{
    [Authorize]
    public class ListUsers : PageModel
    {
        public int TotalNewUsers { get; private set; }

        public int TotalActiveUsers { get; private set; }

        public int TotalLogins { get; private set; }

        public int TotalLockedAccounts { get; private set; }

        public void OnGet()
        {
            this.TotalNewUsers = 2;
            this.TotalActiveUsers = 3;
            this.TotalLogins = 4;
            this.TotalLockedAccounts = 5;
        }
    }
}