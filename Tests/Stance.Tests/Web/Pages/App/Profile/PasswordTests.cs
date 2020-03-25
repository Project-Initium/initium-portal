// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Infrastructure.PageModels;
using Stance.Web.Pages.App.Profile;
using Xunit;

namespace Stance.Tests.Web.Pages.App.Profile
{
    public class PasswordTests
    {
        [Fact]
        public async Task OnPostAsync_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();

            var page = new Password(mediator.Object);
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

            var page = new Password(mediator.Object) { PageModel = new Password.Model() };

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

            var page = new Password(mediator.Object) { PageModel = new Password.Model() };

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Failed, page.PrgState);
        }

        public class Validator
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var model = new Password.Model
                {
                    ConfirmPassword = "new-password",
                    NewPassword = "new-password",
                    OldPassword = "old-password",
                };
                var validator = new Password.Validator();
                var result = validator.Validate(model);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenNewPasswordIsEmpty_ExpectValidationFailure()
            {
                var model = new Password.Model
                {
                    ConfirmPassword = "new-password",
                    NewPassword = string.Empty,
                    OldPassword = "old-password",
                };
                var validator = new Password.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "NewPassword");
            }

            [Fact]
            public void Validate_GivenNewPasswordIsNull_ExpectValidationFailure()
            {
                var model = new Password.Model
                {
                    ConfirmPassword = "new-password",
                    NewPassword = null,
                    OldPassword = "old-password",
                };
                var validator = new Password.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "NewPassword");
            }

            [Fact]
            public void Validate_GivenOldPasswordIsEmpty_ExpectValidationFailure()
            {
                var model = new Password.Model
                {
                    ConfirmPassword = "new-password",
                    NewPassword = "new-password",
                    OldPassword = string.Empty,
                };
                var validator = new Password.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "OldPassword");
            }

            [Fact]
            public void Validate_GivenOldPasswordIsNull_ExpectValidationFailure()
            {
                var model = new Password.Model
                {
                    ConfirmPassword = "new-password",
                    NewPassword = "new-password",
                    OldPassword = null,
                };
                var validator = new Password.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "OldPassword");
            }

            [Fact]
            public void Validate_GivenConfirmPasswordDoesNotMatch_ExpectValidationFailure()
            {
                var model = new Password.Model
                {
                    ConfirmPassword = string.Empty,
                    NewPassword = "new-password",
                    OldPassword = "old-password",
                };
                var validator = new Password.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "ConfirmPassword");
            }
        }
    }
}