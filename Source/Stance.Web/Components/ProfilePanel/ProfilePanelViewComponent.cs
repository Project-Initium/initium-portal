// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Microsoft.AspNetCore.Mvc;
using Stance.Core.Contracts;

namespace Stance.Web.Components.ProfilePanel
{
    public class ProfilePanelViewComponent : ViewComponent
    {
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;

        public ProfilePanelViewComponent(ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ?? throw new ArgumentNullException(nameof(currentAuthenticatedUserProvider));
        }

        public IViewComponentResult Invoke()
        {
            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return this.View(new ProfilePanelViewModel());
            }

            return this.View(new ProfilePanelViewModel(
                currentUser.Value.EmailAddress,
                $"{currentUser.Value.FirstName} {currentUser.Value.LastName}"));
        }
    }
}