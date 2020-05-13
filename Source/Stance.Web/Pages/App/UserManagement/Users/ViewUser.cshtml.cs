// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stance.Core.Contracts;
using Stance.Queries.Contracts.Static;
using Stance.Queries.Static.Models.User;
using Stance.Web.Infrastructure.Attributes;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.App.UserManagement.Users
{
    [ResourceBasedAuthorize("user-view")]
    public class ViewUser : NotificationPageModel
    {
        private readonly IUserQueries _userQueries;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;

        public ViewUser(IUserQueries userQueries, ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            this._userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ?? throw new ArgumentNullException(nameof(currentAuthenticatedUserProvider));
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public DetailedUserModel DetailedUser { get; private set; }

        public bool ViewingSelf { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var maybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (maybe.HasNoValue)
            {
                return this.NotFound();
            }

            if (maybe.Value.UserId == this.Id)
            {
                this.ViewingSelf = true;
            }

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