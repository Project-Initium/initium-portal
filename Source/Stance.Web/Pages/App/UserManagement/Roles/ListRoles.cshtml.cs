// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Stance.Web.Infrastructure.Attributes;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.App.UserManagement.Roles
{
    [ResourceBasedAuthorize("role-list")]
    public class ListRoles : NotificationPageModel
    {
    }
}