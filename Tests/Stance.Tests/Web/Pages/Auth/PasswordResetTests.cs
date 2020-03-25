// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Infrastructure.PageModels;
using Stance.Web.Pages.Auth;
using Xunit;

namespace Stance.Tests.Web.Pages.Auth
{
    public class PasswordResetTests
    {
        [Fact]
        public void OnGet_GivenPageModelIsNull_ExpectTokenSetInPageModel()
        {
            var mediator = new Mock<IMediator>();

            var page = new PasswordReset(mediator.Object) { Token = new string('*', 5) };
            page.OnGet();

            Assert.NotNull(page.PageModel);
            Assert.Equal(new string('*', 5), page.PageModel.Token);
        }

        [Fact]
        public void OnGet_GivenPageModelNotNull_ExpectTokenToBeIgnored()
        {
            var mediator = new Mock<IMediator>();

            var page = new PasswordReset(mediator.Object)
            {
                Token = new string('*', 5),
                PageModel = new PasswordReset.Model { Token = new string('*', 6) },
            };
            page.OnGet();

            Assert.Equal(new string('*', 6), page.PageModel.Token);
        }

        [Fact]
        public async Task OnPost_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();

            var page = new PasswordReset(mediator.Object)
            {
                PageModel = new PasswordReset.Model
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
            mediator.Setup(x => x.Send(It.IsAny<PasswordResetCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>);

            var page = new PasswordReset(mediator.Object)
            {
                PageModel = new PasswordReset.Model(),
            };

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Success, page.PrgState);
        }

        [Fact]
        public async Task OnPost_GivenValidModelStateAndCommandDoesNotExecute_ExpectRedirectToPageResultAndPrgStateSetToFailed()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<PasswordResetCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.SavingChanges)));

            var page = new PasswordReset(mediator.Object)
            {
                PageModel = new PasswordReset.Model(),
            };

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Failed, page.PrgState);
        }

        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var model = new PasswordReset.Model
            {
                Token = new string('*', 5),
                Password = new string('*', 6),
                PasswordConfirmation = new string('*', 6),
            };
            var validator = new PasswordReset.Validator();
            var result = validator.Validate(model);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenPasswordDoesNotMatchPasswordConfirmation_ExpectValidationFailure()
        {
            var model = new PasswordReset.Model
            {
                Token = new string('*', 5),
                Password = new string('*', 6),
                PasswordConfirmation = new string('*', 7),
            };
            var validator = new PasswordReset.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "PasswordConfirmation");
        }

        [Fact]
        public void Validate_GivenPasswordIsEmpty_ExpectValidationFailure()
        {
            var model = new PasswordReset.Model
            {
                Token = new string('*', 5),
                Password = string.Empty,
                PasswordConfirmation = string.Empty,
            };
            var validator = new PasswordReset.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "Password");
        }

        [Fact]
        public void Validate_GivenPasswordIsNull_ExpectValidationFailure()
        {
            var model = new PasswordReset.Model
            {
                Token = new string('*', 5),
                Password = null,
                PasswordConfirmation = null,
            };
            var validator = new PasswordReset.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "Password");
        }

        [Fact]
        public void Validate_GivenTokenIsEmpty_ExpectValidationFailure()
        {
            var model = new PasswordReset.Model
            {
                Token = string.Empty,
                Password = new string('*', 6),
                PasswordConfirmation = new string('*', 6),
            };
            var validator = new PasswordReset.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "Token");
        }

        [Fact]
        public void Validate_GivenTokenIsNull_ExpectValidationFailure()
        {
            var model = new PasswordReset.Model
            {
                Token = null,
                Password = new string('*', 6),
                PasswordConfirmation = new string('*', 6),
            };
            var validator = new PasswordReset.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "Token");
        }
    }
}