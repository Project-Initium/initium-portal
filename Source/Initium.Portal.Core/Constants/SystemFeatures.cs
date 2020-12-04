// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;

namespace Initium.Portal.Core.Constants
{
    public enum SystemFeatures
    {
        [Display(Name = "System Alerts")]
        SystemAlerts,
        [Display(Name = "MFA App")]
        MfaApp,
        [Display(Name = "MFA Device")]
        MfaDevice,
        [Display(Name = "User Notifications")]
        UserNotifications,
    }
}