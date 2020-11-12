// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Initium.Portal.Core;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Queries.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Management.Components.TopNav
{
    public class TopNavViewComponent : ViewComponent
    {
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IUserNotificationQueryService _userNotificationQueryService;

        public TopNavViewComponent(ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider, IUserNotificationQueryService userNotificationQueryService)
        {
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ?? throw new ArgumentNullException(nameof(currentAuthenticatedUserProvider));
            this._userNotificationQueryService = userNotificationQueryService ?? throw new ArgumentNullException(nameof(userNotificationQueryService));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasValue && currentUser.Value is AuthenticatedUser user)
            {
                return this.View(new TopNavViewModel(
                    user.EmailAddress,
                    $"{user.FirstName} {user.LastName}",
                    await this._userNotificationQueryService.AnyUnread()));
            }

            return this.View(new TopNavViewModel());
        }
    }
}