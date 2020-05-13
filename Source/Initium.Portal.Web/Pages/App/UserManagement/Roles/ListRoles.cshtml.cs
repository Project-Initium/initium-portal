// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Web.Infrastructure.Attributes;
using Initium.Portal.Web.Infrastructure.PageModels;

namespace Initium.Portal.Web.Pages.App.UserManagement.Roles
{
    [ResourceBasedAuthorize("role-list")]
    public class ListRoles : NotificationPageModel
    {
    }
}