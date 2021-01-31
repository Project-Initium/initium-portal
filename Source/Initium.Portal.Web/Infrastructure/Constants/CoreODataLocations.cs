﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Initium.Portal.Web.Infrastructure.Constants
{
    public static class CoreODataLocations
    {
        public const string Root = "/odata/";
        public const string UserFilteredListing = "/odata/users/filtered()";
        public const string UserFilteredExport = "/odata/users/filteredexport()";
        public const string RoleFilteredExport = "/odata/role/role.filteredexport";
        public const string RoleFilteredListing = "/odata/role/role.filtered";
        public const string UserNotificationFilteredExport = "/odata/userNotification/userNotification.filteredexport";
        public const string UserNotificationFilteredListing = "/odata/userNotification/userNotification.filtered";

        public const string SystemAlertFilteredExport = "/odata/systemAlert/systemAlert.filteredexport";

        public const string SystemAlertFilteredListing = "/odata/systemAlert/systemAlert.filtered";
    }
}