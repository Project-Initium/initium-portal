// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Initium.Portal.Core;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Queries.Contracts.Static;
using Initium.Portal.Queries.Static.Models;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Pages.App.Profile;
using MaybeMonad;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.App.Profile
{
    public class AuthenticatorAppTests
    {
        [Fact]
        public async Task OnGetAsync_GivenNoUserAppearsToBeAuthenticate_ExpectErroredPrgState()
        {
            var userQueries = new Mock<IUserQueries>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);

            var page = new AuthenticatorApp(userQueries.Object, currentAuthenticatedUserProvider.Object);

            await page.OnGetAsync();

            Assert.Equal(PrgState.InError, page.PrgState);
        }

        [Fact]
        public async Task OnGetAsync_GivenUserHasAnAppEnrolled_ExpectSetupToBeTrue()
        {
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.CheckForPresenceOfAuthAppForCurrentUser())
                .ReturnsAsync(
                    () => new StatusCheckModel(true));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(() =>
                {
                    ISystemUser user = new AuthenticatedUser(TestVariables.UserId, "email-address", "first-name", "last-name");
                    return Maybe.From(user);
                });

            var page = new AuthenticatorApp(userQueries.Object, currentAuthenticatedUserProvider.Object);

            await page.OnGetAsync();

            Assert.True(page.IsSetup);
        }

        public class ValidatorTests
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var cmd = new AuthenticatorApp.Model { Password = "password", Code = "code" };
                var validator = new AuthenticatorApp.Validator();
                var result = validator.Validate(cmd);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenCodeIsEmpty_ExpectValidationFailure()
            {
                var cmd = new AuthenticatorApp.Model { Password = "password", Code = string.Empty };
                var validator = new AuthenticatorApp.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Code");
            }

            [Fact]
            public void Validate_GivenCodeIsNull_ExpectValidationFailure()
            {
                var cmd = new AuthenticatorApp.Model { Password = "password", Code = null };
                var validator = new AuthenticatorApp.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Code");
            }

            [Fact]
            public void Validate_GivenPasswordIsEmpty_ExpectValidationFailure()
            {
                var cmd = new AuthenticatorApp.Model { Password = string.Empty, Code = "code" };
                var validator = new AuthenticatorApp.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Password");
            }

            [Fact]
            public void Validate_GivenPasswordIsNull_ExpectValidationFailure()
            {
                var cmd = new AuthenticatorApp.Model { Password = null, Code = "code" };
                var validator = new AuthenticatorApp.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Password");
            }
        }
    }
}