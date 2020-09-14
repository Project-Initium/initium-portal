// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Models.User;
using Initium.Portal.Web.Infrastructure.Attributes;
using Initium.Portal.Web.Infrastructure.PageModels;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Pages.App.UserManagement.Users
{
    [ResourceBasedAuthorize("user-view")]
    public class ViewUser : NotificationPageModel
    {
        private readonly IUserQueryService _userQueryService;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;

        public ViewUser(IUserQueryService userQueryService, ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            this._userQueryService = userQueryService ?? throw new ArgumentNullException(nameof(userQueryService));
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

            var userMaybe = await this._userQueryService.GetDetailsOfUserById(this.Id);
            if (userMaybe.HasNoValue)
            {
                return this.NotFound();
            }

            this.DetailedUser = userMaybe.Value;

            return this.Page();
        }
    }
}