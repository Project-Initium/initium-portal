// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Linq;
using FluentValidation.Validators;
using Stance.Core.Settings;

namespace Stance.Core.Validation
{
    public class PasswordValidator : PropertyValidator
    {
        private readonly SecuritySettings _securitySettings;

        public PasswordValidator(SecuritySettings securitySettings)
            : base("{Message}")
        {
            this._securitySettings = securitySettings;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (!(context.PropertyValue is string password))
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < this._securitySettings.PasswordRequirements.RequiredLength)
            {
                context.MessageFormatter.AppendArgument(
                    "Message",
                    $"Passwords must be at least {this._securitySettings.PasswordRequirements.RequiredLength} characters.");
                return false;
            }

            if (this._securitySettings.PasswordRequirements.RequireNonAlphanumeric && password.All(IsLetterOrDigit))
            {
                context.MessageFormatter.AppendArgument("Message", "Passwords must have at least one non alphanumeric character.");
                return false;
            }

            if (this._securitySettings.PasswordRequirements.RequireDigit && !password.Any(IsDigit))
            {
                context.MessageFormatter.AppendArgument("Message", "Passwords must have at least one digit ('0'-'9').");
                return false;
            }

            if (this._securitySettings.PasswordRequirements.RequireLowercase && !password.Any(IsLower))
            {
                context.MessageFormatter.AppendArgument("Message", "Passwords must have at least one lowercase ('a'-'z').");
                return false;
            }

            if (this._securitySettings.PasswordRequirements.RequireUppercase && !password.Any(IsUpper))
            {
                context.MessageFormatter.AppendArgument("Message", "Passwords must have at least one uppercase ('A'-'Z').");
                return false;
            }

            if (this._securitySettings.PasswordRequirements.RequiredUniqueChars >= 1 && password.Distinct().Count() < this._securitySettings.PasswordRequirements.RequiredUniqueChars)
            {
                context.MessageFormatter.AppendArgument("Message", $"Passwords must use at least {this._securitySettings.PasswordRequirements.RequiredUniqueChars} different characters.");
                return false;
            }

            return true;
        }

        private static bool IsLetterOrDigit(char c)
        {
            return IsUpper(c) || IsLower(c) || IsDigit(c);
        }

        private static bool IsUpper(char c)
        {
            return c >= 'A' && c <= 'Z';
        }

        private static bool IsLower(char c)
        {
            return c >= 'a' && c <= 'z';
        }

        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }
    }
}