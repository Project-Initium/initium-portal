// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.CommandResults.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Infrastructure.PageModels;
using Stance.Web.Pages.Auth;
using Xunit;

namespace Stance.Tests.Web.Pages.Auth
{
    public class ValidateEmailMfaCodeTests
    {
        [Fact]
        public async Task OnPost_GivenCommandFails_ExpectRedirectToPageResultToSamePageAndPrgStateSet()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.Send(
                    It.IsAny<ValidateEmailMfaCodeAgainstCurrentUserCommand>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(() =>
                    Result.Fail<ValidateEmailMfaCodeAgainstCurrentUserCommandResult, ErrorData>(
                        new ErrorData(ErrorCodes.SavingChanges)));

            var page = new ValidateEmailMfaCode(mediator.Object) { PageModel = new ValidateEmailMfaCode.Model() };

            var result = await page.OnPost();

            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.InError, page.PrgState);
        }

        [Fact]
        public async Task OnPost_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();

            var page = new ValidateEmailMfaCode(mediator.Object);
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPost();
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var model = new ValidateEmailMfaCode.Model { Code = new string('*', 6) };
            var validator = new ValidateEmailMfaCode.Validator();
            var result = validator.Validate(model);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenCodeIsEmpty_ExpectValidationFailure()
        {
            var model = new ValidateEmailMfaCode.Model { Code = string.Empty };
            var validator = new ValidateEmailMfaCode.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "Code");
        }

        [Fact]
        public void Validate_GivenCodeIsNull_ExpectValidationFailure()
        {
            var model = new ValidateEmailMfaCode.Model { Code = null };
            var validator = new ValidateEmailMfaCode.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "Code");
        }
    }
}