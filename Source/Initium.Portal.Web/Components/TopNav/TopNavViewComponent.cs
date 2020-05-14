// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core;
using Initium.Portal.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Components.TopNav
{
    public class TopNavViewComponent : ViewComponent
    {
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;

        public TopNavViewComponent(ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ?? throw new ArgumentNullException(nameof(currentAuthenticatedUserProvider));
        }

        public IViewComponentResult Invoke()
        {
            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasValue && currentUser.Value is AuthenticatedUser user)
            {
                return this.View(new TopNavViewModel(
                    user.EmailAddress,
                    $"{user.FirstName} {user.LastName}"));
            }

            return this.View(new TopNavViewModel());
        }
    }
}