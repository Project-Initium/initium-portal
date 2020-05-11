// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using MaybeMonad;
using Moq;
using Stance.Queries.Contracts.Static;
using Stance.Queries.Static.Models.User;
using Stance.Web.Pages.App.Profile;
using Xunit;

namespace Stance.Tests.Web.Pages.App.Profile
{
    public class SecurityKeysTests
    {
        [Fact]
        public async Task OnGet_GivenNoDevices_ExpectEmptyDevicesList()
        {
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetDeviceInfoForCurrentUser()).ReturnsAsync(Maybe<List<DeviceInfo>>.Nothing);

            var page = new SecurityKeys(userQueries.Object);
            await page.OnGet();

            Assert.Empty(page.DeviceInfos);
        }

        [Fact]
        public async Task OnGet_GivenUserHasDevices_ExpectPopulatedDevicesList()
        {
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetDeviceInfoForCurrentUser())
                .ReturnsAsync(Maybe.From(new List<DeviceInfo>
                {
                    new DeviceInfo(
                        TestVariables.AuthenticatorDeviceId,
                        "name",
                        TestVariables.Now.AddMinutes(-60),
                        TestVariables.Now),
                }));

            var page = new SecurityKeys(userQueries.Object);
            await page.OnGet();

            Assert.Single(page.DeviceInfos);
        }

        public class ValidatorTests
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var cmd = new SecurityKeys.Model { Name = "name", Password = "password" };
                var validator = new SecurityKeys.Validator();
                var result = validator.Validate(cmd);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenNameIsEmpty_ExpectValidationFailure()
            {
                var cmd = new SecurityKeys.Model { Name = string.Empty, Password = "password" };
                var validator = new SecurityKeys.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Name");
            }

            [Fact]
            public void Validate_GivenNameIsNull_ExpectValidationFailure()
            {
                var cmd = new SecurityKeys.Model { Name = null, Password = "password" };
                var validator = new SecurityKeys.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Name");
            }

            [Fact]
            public void Validate_GivenPasswordIsEmpty_ExpectValidationFailure()
            {
                var cmd = new SecurityKeys.Model { Name = "name", Password = string.Empty };
                var validator = new SecurityKeys.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Password");
            }

            [Fact]
            public void Validate_GivenPasswordIsNull_ExpectValidationFailure()
            {
                var cmd = new SecurityKeys.Model { Name = "name", Password = null };
                var validator = new SecurityKeys.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Password");
            }
        }
    }
}