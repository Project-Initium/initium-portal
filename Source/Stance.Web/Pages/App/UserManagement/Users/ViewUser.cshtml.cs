// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stance.Queries.Contracts;
using Stance.Queries.Models.User;

namespace Stance.Web.Pages.App.UserManagement.Users
{
    public class ViewUser : PageModel
    {
        private readonly IUserQueries _userQueries;

        public ViewUser(IUserQueries userQueries)
        {
            this._userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public DetailedUserModel DetailedUser { get; private set; }

        public async Task<IActionResult> OnGet()
        {
            var userMaybe = await this._userQueries.GetDetailsOfUserById(this.Id);
            if (userMaybe.HasNoValue)
            {
                return this.NotFound();
            }

            this.DetailedUser = userMaybe.Value;

            return this.Page();
        }
    }
}