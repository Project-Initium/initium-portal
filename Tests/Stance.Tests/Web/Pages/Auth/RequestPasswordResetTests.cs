// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Infrastructure.PageModels;
using Stance.Web.Pages.Auth;
using Xunit;

namespace Stance.Tests.Web.Pages.Auth
{
    public class RequestPasswordResetTests
    {
        [Fact]
        public async Task OnPost_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();

            var page = new RequestPasswordReset(mediator.Object) { PageModel = new RequestPasswordReset.Model() };
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPost_GivenValidModelState_ExpectRedirectToPageResultToTheSamePage()
        {
            var mediator = new Mock<IMediator>();

            var page = new RequestPasswordReset(mediator.Object) { PageModel = new RequestPasswordReset.Model() };

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);

            Assert.Equal(PrgState.Success, page.PrgState);
            mediator.Verify(
                x => x.Send(It.IsAny<RequestPasswordResetCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        public class ValidatorTests
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var model = new RequestPasswordReset.Model { EmailAddress = "a@b.com" };
                var validator = new RequestPasswordReset.Validator();
                var result = validator.Validate(model);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenEmailAddressIsEmpty_ExpectValidationFailure()
            {
                var model = new RequestPasswordReset.Model { EmailAddress = string.Empty };
                var validator = new RequestPasswordReset.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
            }

            [Fact]
            public void Validate_GivenEmailAddressIsNotValidEmailAddress_ExpectValidationFailure()
            {
                var model = new RequestPasswordReset.Model { EmailAddress = "email-address" };
                var validator = new RequestPasswordReset.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
            }

            [Fact]
            public void Validate_GivenEmailAddressIsNull_ExpectValidationFailure()
            {
                var model = new RequestPasswordReset.Model { EmailAddress = null };
                var validator = new RequestPasswordReset.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
            }
        }
    }
}