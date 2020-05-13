// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Initium.Portal.Core.Settings
{
    public class SecuritySettings
    {
        public int AllowedAttempts { get; set; }

        public int PasswordTokenLifetime { get; set; }

        public string SiteName { get; set; }

        public string ServerDomain { get; set; }

        public string Origin { get; set; }

        public int AccountVerificationTokenLifetime { get; set; }

        public int PasswordRequiredLength { get; set; }

        public PasswordRequirement PasswordRequirements { get; set; }

        public int HistoricalLimit { get; set; }

        public class PasswordRequirement
        {
            public int RequiredLength { get; set; }

            public bool RequireNonAlphanumeric { get; set; }

            public bool RequireDigit { get; set; }

            public bool RequireLowercase { get; set; }

            public bool RequireUppercase { get; set; }

            public int RequiredUniqueChars { get; set; }
        }
    }
}