// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentValidation;
using Initium.Portal.Core.Settings;
using Initium.Portal.Core.Validation;
using Xunit;

namespace Initium.Portal.Tests.Core.Validation
{
    public class PasswordValidatorTests
    {
        [Fact]
        public void Validate_GivenFieldIsNotString_ExpectValidationSuccess()
        {
            var model = new DummyModel();
            var validator = new InvalidDummyModelValidator(new SecuritySettings
            {
                PasswordRequirements = new SecuritySettings.PasswordRequirement
                {
                    RequireDigit = true,
                    RequiredLength = 8,
                    RequireLowercase = true,
                    RequireUppercase = true,
                    RequiredUniqueChars = 8,
                    RequireNonAlphanumeric = true,
                },
            });

            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void Validate_GivenFieldIsNull_ExpectValidationFailure()
        {
            var model = new DummyModel
            {
                Prop2 = null,
            };
            var validator = new DummyModelValidator(new SecuritySettings
            {
                PasswordRequirements = new SecuritySettings.PasswordRequirement
                {
                    RequireDigit = true,
                    RequiredLength = 8,
                    RequireLowercase = true,
                    RequireUppercase = true,
                    RequiredUniqueChars = 8,
                    RequireNonAlphanumeric = true,
                },
            });

            var result = validator.Validate(model);
            Assert.Single(result.Errors);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validate_GivenFieldIsEmpty_ExpectValidationFailure()
        {
            var model = new DummyModel
            {
                Prop2 = string.Empty,
            };
            var validator = new DummyModelValidator(new SecuritySettings
            {
                PasswordRequirements = new SecuritySettings.PasswordRequirement
                {
                    RequireDigit = false,
                    RequiredLength = 8,
                    RequireLowercase = false,
                    RequireUppercase = false,
                    RequiredUniqueChars = 0,
                    RequireNonAlphanumeric = false,
                },
            });

            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Passwords must be at least 8 characters.");
        }

        [Fact]
        public void Validate_GivenRequiredLengthIsSetAndPasswordLengthIsLess_ExpectValidationFailure()
        {
            var model = new DummyModel
            {
                Prop2 = "aabcdef",
            };
            var validator = new DummyModelValidator(new SecuritySettings
            {
                PasswordRequirements = new SecuritySettings.PasswordRequirement()
                {
                    RequiredLength = 8,
                    RequireDigit = false,
                    RequireLowercase = false,
                    RequireUppercase = false,
                    RequiredUniqueChars = 0,
                    RequireNonAlphanumeric = false,
                },
            });

            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Passwords must be at least 8 characters.");
        }

        [Fact]
        public void Validate_GivenRequireDigitIsSetAndPasswordHasNoDigits_ExpectValidationFailure()
        {
            var model = new DummyModel
            {
                Prop2 = "aabcdefg",
            };
            var validator = new DummyModelValidator(new SecuritySettings
            {
                PasswordRequirements = new SecuritySettings.PasswordRequirement
                {
                    RequiredLength = 8,
                    RequireDigit = true,
                    RequireLowercase = false,
                    RequireUppercase = false,
                    RequiredUniqueChars = 0,
                    RequireNonAlphanumeric = false,
                },
            });

            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Passwords must have at least one digit ('0'-'9').");
        }

        [Fact]
        public void Validate_GivenRequireLowercaseIsSetAndPasswordHasNoLowercase_ExpectValidationFailure()
        {
            var model = new DummyModel
            {
                Prop2 = "AABCDEF1",
            };
            var validator = new DummyModelValidator(new SecuritySettings
            {
                PasswordRequirements = new SecuritySettings.PasswordRequirement
                {
                    RequiredLength = 8,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireUppercase = false,
                    RequiredUniqueChars = 0,
                    RequireNonAlphanumeric = false,
                },
            });

            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Passwords must have at least one lowercase ('a'-'z').");
        }

        [Fact]
        public void Validate_GivenRequireUppercaseIsSetAndPasswordHasNoUppercase_ExpectValidationFailure()
        {
            var model = new DummyModel
            {
                Prop2 = "aabcdef1",
            };
            var validator = new DummyModelValidator(new SecuritySettings
            {
                PasswordRequirements = new SecuritySettings.PasswordRequirement
                {
                    RequiredLength = 8,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireUppercase = true,
                    RequiredUniqueChars = 0,
                    RequireNonAlphanumeric = false,
                },
            });

            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Passwords must have at least one uppercase ('A'-'Z').");
        }

        [Fact]
        public void Validate_GivenRequiredUniqueCharsIsSetAndPasswordDoesNotHaveTheRequiredNumber_ExpectValidationFailure()
        {
            var model = new DummyModel
            {
                Prop2 = "AAbcdef1",
            };
            var validator = new DummyModelValidator(new SecuritySettings
            {
                PasswordRequirements = new SecuritySettings.PasswordRequirement
                {
                    RequiredLength = 8,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireUppercase = true,
                    RequiredUniqueChars = 8,
                    RequireNonAlphanumeric = false,
                },
            });

            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Passwords must use at least 8 different characters.");
        }

        [Fact]
        public void Validate_GivenRequireNonAlphanumericsIsSetAndPasswordHasOnlyHasAlphanumeric_ExpectValidationFailure()
        {
            var model = new DummyModel
            {
                Prop2 = "Abcdefg1",
            };
            var validator = new DummyModelValidator(new SecuritySettings
            {
                PasswordRequirements = new SecuritySettings.PasswordRequirement
                {
                    RequiredLength = 8,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireUppercase = true,
                    RequiredUniqueChars = 8,
                    RequireNonAlphanumeric = true,
                },
            });

            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(result.Errors, x => x.ErrorMessage == "Passwords must have at least one non alphanumeric character.");
        }

        [Fact]
        public void Validate_GivenPasswordMeetsAllRequirements_ExpectValidationSuccess()
        {
            var model = new DummyModel
            {
                Prop2 = "Abcd3fg!",
            };
            var validator = new DummyModelValidator(new SecuritySettings
            {
                PasswordRequirements = new SecuritySettings.PasswordRequirement
                {
                    RequiredLength = 8,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireUppercase = true,
                    RequiredUniqueChars = 8,
                    RequireNonAlphanumeric = true,
                },
            });

            var result = validator.Validate(model);
            Assert.True(result.IsValid);
        }

        private class DummyModel
        {
            public int Prop1 { get; } = 0;

            public string Prop2 { get; set; }
        }

        private class DummyModelValidator : AbstractValidator<DummyModel>
        {
            public DummyModelValidator(SecuritySettings securitySettings)
            {
                this.RuleFor(x => x.Prop2)
                    .SetValidator(new PasswordValidator(securitySettings));
            }
        }

        private class InvalidDummyModelValidator : AbstractValidator<DummyModel>
        {
            public InvalidDummyModelValidator(SecuritySettings securitySettings)
            {
                this.RuleFor(x => x.Prop1)
                    .SetValidator(new PasswordValidator(securitySettings));
            }
        }
    }
}