// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Core.Settings;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Pages.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.Auth
{
    public class AccountVerificationTests
    {
        [Fact]
        public void OnGet_GivenPageModelIsNull_ExpectTokenSetInPageModel()
        {
            var mediator = new Mock<IMediator>();

            var page = new AccountVerification(mediator.Object) { Token = "token" };
            page.OnGet();

            Assert.NotNull(page.PageModel);
            Assert.Equal("token", page.PageModel.Token);
        }

        [Fact]
        public void OnGet_GivenPageModelNotNull_ExpectTokenToBeIgnored()
        {
            var mediator = new Mock<IMediator>();

            var page = new AccountVerification(mediator.Object)
            {
                Token = "token",
                PageModel = new AccountVerification.Model { Token = "model-token" },
            };
            page.OnGet();

            Assert.Equal("model-token", page.PageModel.Token);
        }

        [Fact]
        public async Task OnPost_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();

            var page = new AccountVerification(mediator.Object)
            {
                PageModel = new AccountVerification.Model
                {
                    Token = "token",
                },
            };
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPost_GivenValidModelStateAndCommandExecutes_ExpectRedirectToPageResultAndPrgStateSetToSuccess()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<VerifyAccountAndSetPasswordCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>);

            var page = new AccountVerification(mediator.Object)
            {
                PageModel = new AccountVerification.Model(),
            };

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Success, page.PrgState);
        }

        [Fact]
        public async Task OnPost_GivenValidModelStateAndCommandDoesNotExecute_ExpectRedirectToPageResultAndPrgStateSetToFailed()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<VerifyAccountAndSetPasswordCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.SavingChanges)));

            var page = new AccountVerification(mediator.Object)
            {
                PageModel = new AccountVerification.Model(),
            };

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
                var model = new AccountVerification.Model
                {
                    Token = "token",
                    Password = "password",
                    PasswordConfirmation = "password",
                };
                var validator = new AccountVerification.Validator(securitySettings.Object);
                var result = validator.Validate(model);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenPasswordDoesNotMatchPasswordConfirmation_ExpectValidationFailure()
            {
                var securitySettings = new Mock<IOptions<SecuritySettings>>();
                securitySettings.Setup(x => x.Value).Returns(new SecuritySettings
                {
                    PasswordRequirements = new SecuritySettings.PasswordRequirement(),
                });
                var model = new AccountVerification.Model
                {
                    Token = "token",
                    Password = "password",
                    PasswordConfirmation = "new-password",
                };
                var validator = new AccountVerification.Validator(securitySettings.Object);
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "PasswordConfirmation");
            }

            [Fact]
            public void Validate_GivenPasswordIsEmpty_ExpectValidationFailure()
            {
                var securitySettings = new Mock<IOptions<SecuritySettings>>();
                securitySettings.Setup(x => x.Value).Returns(new SecuritySettings
                {
                    PasswordRequirements = new SecuritySettings.PasswordRequirement(),
                });
                var model = new AccountVerification.Model
                {
                    Token = "token",
                    Password = string.Empty,
                    PasswordConfirmation = string.Empty,
                };
                var validator = new AccountVerification.Validator(securitySettings.Object);
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "Password");
            }

            [Fact]
            public void Validate_GivenPasswordIsNull_ExpectValidationFailure()
            {
                var securitySettings = new Mock<IOptions<SecuritySettings>>();
                securitySettings.Setup(x => x.Value).Returns(new SecuritySettings
                {
                    PasswordRequirements = new SecuritySettings.PasswordRequirement(),
                });
                var model = new AccountVerification.Model
                {
                    Token = "token",
                    Password = null,
                    PasswordConfirmation = null,
                };
                var validator = new AccountVerification.Validator(securitySettings.Object);
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "Password");
            }

            [Fact]
            public void Validate_GivenTokenIsEmpty_ExpectValidationFailure()
            {
                var securitySettings = new Mock<IOptions<SecuritySettings>>();
                securitySettings.Setup(x => x.Value).Returns(new SecuritySettings
                {
                    PasswordRequirements = new SecuritySettings.PasswordRequirement(),
                });
                var model = new AccountVerification.Model
                {
                    Token = string.Empty,
                    Password = "password",
                    PasswordConfirmation = "password",
                };
                var validator = new AccountVerification.Validator(securitySettings.Object);
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "Token");
            }

            [Fact]
            public void Validate_GivenTokenIsNull_ExpectValidationFailure()
            {
                var securitySettings = new Mock<IOptions<SecuritySettings>>();
                securitySettings.Setup(x => x.Value).Returns(new SecuritySettings
                {
                    PasswordRequirements = new SecuritySettings.PasswordRequirement(),
                });
                var model = new AccountVerification.Model
                {
                    Token = null,
                    Password = "password",
                    PasswordConfirmation = "password",
                };
                var validator = new AccountVerification.Validator(securitySettings.Object);
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "Token");
            }
        }
    }
}