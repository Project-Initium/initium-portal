// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Stance.Core.Settings
{
    public class SecuritySettings
    {
        public int AllowedAttempts { get; set; }

        public bool EnforceMfa { get; set; }

        public int PasswordTokenLifetime { get; set; }

        public string SiteName { get; set; }
    }
}