// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Caching;
using Initium.Portal.Core.Domain;
using Initium.Portal.Core.Settings;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.EventHandlers.Models;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Pages.Auth;
using MaybeMonad;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NodaTime;
using NodaTime.Extensions;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.Auth
{
    public class AccountVerificationTests
    {
        [Fact]
        public void OnGet_GivenTokenFailsToDeserialize_ExpectTokenError()
        {
            var dataSerializer = new Mock<IDataSerializer>();
            dataSerializer.Setup(x => x.DeserializeFromBase64<SecurityToken>(It.IsAny<string>()))
                .Returns(Maybe<SecurityToken>.Nothing);

            var page = new AccountVerification(
                Mock.Of<IMediator>(),
                dataSerializer.Object,
                Mock.Of<IClock>()) { Token = "token" };
            page.OnGet();

            Assert.True(page.TokenError);
            Assert.Null(page.PageModel);
        }

        [Fact]
        public void OnGet_GivenTokenHasExpired_ExpectTokenError()
        {
            var dataSerializer = new Mock<IDataSerializer>();
            dataSerializer.Setup(x => x.DeserializeFromBase64<SecurityToken>(It.IsAny<string>()))
                .Returns(Maybe.From(new SecurityToken(
                    TestVariables.SecurityTokenMappingId,
                    new DateTimeOffset(TestVariables.Now.AddDays(-1)).ToUnixTimeSeconds())));

            var clock = new Mock<IClock>();
            clock.Setup(x => x.GetCurrentInstant())
                .Returns(TestVariables.Now.ToInstant());

            var page = new AccountVerification(
                Mock.Of<IMediator>(),
                dataSerializer.Object,
                clock.Object) { Token = "token" };
            page.OnGet();

            Assert.True(page.TokenError);
            Assert.Null(page.PageModel);
        }

        [Fact]
        public void OnGet_GivenPageModelsNull_ExpectTokenToBeSet()
        {
            var dataSerializer = new Mock<IDataSerializer>();
            dataSerializer.Setup(x => x.DeserializeFromBase64<SecurityToken>(It.IsAny<string>()))
                .Returns(Maybe.From(new SecurityToken(
                    TestVariables.SecurityTokenMappingId,
                    new DateTimeOffset(TestVariables.Now.AddDays(1)).ToUnixTimeSeconds())));

            var clock = new Mock<IClock>();
            clock.Setup(x => x.GetCurrentInstant())
                .Returns(TestVariables.Now.ToInstant());

            var page = new AccountVerification(
                Mock.Of<IMediator>(),
                dataSerializer.Object,
                clock.Object)
            {
                Token = "token",
            };
            page.OnGet();

            Assert.Equal(TestVariables.SecurityTokenMappingId, page.PageModel.Token);
        }

        [Fact]
        public void OnGet_GivenPageModelNotNull_ExpectTokenToBeIgnored()
        {
            var dataSerializer = new Mock<IDataSerializer>();
            dataSerializer.Setup(x => x.DeserializeFromBase64<SecurityToken>(It.IsAny<string>()))
                .Returns(Maybe.From(new SecurityToken(
                    Guid.NewGuid(),
                    new DateTimeOffset(TestVariables.Now.AddDays(1)).ToUnixTimeSeconds())));

            var clock = new Mock<IClock>();
            clock.Setup(x => x.GetCurrentInstant())
                .Returns(TestVariables.Now.ToInstant());

            var page = new AccountVerification(
                Mock.Of<IMediator>(),
                dataSerializer.Object,
                clock.Object)
            {
                Token = "token",
                PageModel = new AccountVerification.Model
                {
                    Token = TestVariables.SecurityTokenMappingId,
                },
            };
            page.OnGet();

            Assert.Equal(TestVariables.SecurityTokenMappingId, page.PageModel.Token);
        }

        [Fact]
        public async Task OnPost_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();

            var page = new AccountVerification(mediator.Object, Mock.Of<IDataSerializer>(), Mock.Of<IClock>())
            {
                PageModel = new AccountVerification.Model
                {
                    Token = TestVariables.SecurityTokenMappingId,
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

            var page = new AccountVerification(mediator.Object, Mock.Of<IDataSerializer>(), Mock.Of<IClock>())
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

            var page = new AccountVerification(mediator.Object, Mock.Of<IDataSerializer>(), Mock.Of<IClock>())
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
                    Token = TestVariables.SecurityTokenMappingId,
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
                    Token = TestVariables.SecurityTokenMappingId,
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
                    Token = TestVariables.SecurityTokenMappingId,
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
                    Token = TestVariables.SecurityTokenMappingId,
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
                    Token = Guid.Empty,
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