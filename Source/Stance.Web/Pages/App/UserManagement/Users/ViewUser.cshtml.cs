// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stance.Queries.Contracts.Static;
using Stance.Queries.Static.Models.User;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.App.UserManagement.Users
{
    public class ViewUser : NotificationPageModel
    {
        private readonly IUserQueries _userQueries;

        public ViewUser(IUserQueries userQueries)
        {
            this._userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public DetailedUserModel DetailedUser { get; private set; }

        public async Task<IActionResult> OnGetAsync()
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