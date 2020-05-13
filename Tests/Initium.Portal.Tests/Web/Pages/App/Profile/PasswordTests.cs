// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Core.Settings;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Pages.App.Profile;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.App.Profile
{
    public class PasswordTests
    {
        [Fact]
        public async Task OnPostAsync_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();
            var securitySettings = new Mock<IOptions<SecuritySettings>>();

            var page = new Password(mediator.Object, securitySettings.Object);
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPost_GivenCommandExecutesSuccessfully_ExpectRedirectToPageResultAndPrgStateOfSuccess()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ChangePasswordCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>);

            var securitySettings = new Mock<IOptions<SecuritySettings>>();

            var page = new Password(mediator.Object, securitySettings.Object) { PageModel = new Password.Model() };

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Success, page.PrgState);
        }

        [Fact]
        public async Task OnPost_GivenCommandFailsToExecute_ExpectRedirectToPageResultAndPrgStateOfFailed()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ChangePasswordCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.SavingChanges)));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();

            var page = new Password(mediator.Object, securitySettings.Object) { PageModel = new Password.Model() };

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Failed, page.PrgState);
        }

        public class ValidatorTests
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var securitySettings = new Mock<IOptions<SecuritySettings>>();
                securitySettings.Setup(x => x.Value).Returns(new SecuritySettings
                {
                    PasswordRequirements = new SecuritySettings.PasswordRequirement(),
                });
                var model = new Password.Model
                {
                    ConfirmPassword = "new-password",
                    NewPassword = "new-password",
                    OldPassword = "old-password",
                };
                var validator = new Password.Validator(securitySettings.Object);
                var result = validator.Validate(model);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenNewPasswordIsEmpty_ExpectValidationFailure()
            {
                var securitySettings = new Mock<IOptions<SecuritySettings>>();
                securitySettings.Setup(x => x.Value).Returns(new SecuritySettings
                {
                    PasswordRequirements = new SecuritySettings.PasswordRequirement(),
                });
                var model = new Password.Model
                {
                    ConfirmPassword = "new-password",
                    NewPassword = string.Empty,
                    OldPassword = "old-password",
                };
                var validator = new Password.Validator(securitySettings.Object);
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "NewPassword");
            }

            [Fact]
            public void Validate_GivenNewPasswordIsNull_ExpectValidationFailure()
            {
                var securitySettings = new Mock<IOptions<SecuritySettings>>();
                securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());
                var model = new Password.Model
                {
                    ConfirmPassword = "new-password",
                    NewPassword = null,
                    OldPassword = "old-password",
                };
                var validator = new Password.Validator(securitySettings.Object);
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "NewPassword");
            }

            [Fact]
            public void Validate_GivenOldPasswordIsEmpty_ExpectValidationFailure()
            {
                var securitySettings = new Mock<IOptions<SecuritySettings>>();
                securitySettings.Setup(x => x.Value).Returns(new SecuritySettings
                {
                    PasswordRequirements = new SecuritySettings.PasswordRequirement(),
                });
                var model = new Password.Model
                {
                    ConfirmPassword = "new-password",
                    NewPassword = "new-password",
                    OldPassword = string.Empty,
                };
                var validator = new Password.Validator(securitySettings.Object);
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "OldPassword");
            }

            [Fact]
            public void Validate_GivenOldPasswordIsNull_ExpectValidationFailure()
            {
                var securitySettings = new Mock<IOptions<SecuritySettings>>();
                securitySettings.Setup(x => x.Value).Returns(new SecuritySettings
                {
                    PasswordRequirements = new SecuritySettings.PasswordRequirement(),
                });
                var model = new Password.Model
                {
                    ConfirmPassword = "new-password",
                    NewPassword = "new-password",
                    OldPassword = null,
                };
                var validator = new Password.Validator(securitySettings.Object);
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "OldPassword");
            }

            [Fact]
            public void Validate_GivenConfirmPasswordDoesNotMatch_ExpectValidationFailure()
            {
                var securitySettings = new Mock<IOptions<SecuritySettings>>();
                securitySettings.Setup(x => x.Value).Returns(new SecuritySettings
                {
                    PasswordRequirements = new SecuritySettings.PasswordRequirement(),
                });
                var model = new Password.Model
                {
                    ConfirmPassword = string.Empty,
                    NewPassword = "new-password",
                    OldPassword = "old-password",
                };
                var validator = new Password.Validator(securitySettings.Object);
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "ConfirmPassword");
            }
        }
    }
}