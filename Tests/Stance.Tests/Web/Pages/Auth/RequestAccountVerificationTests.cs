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
    public class RequestAccountVerificationTests
    {
        [Fact]
        public async Task OnPost_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();

            var page = new RequestAccountVerification(mediator.Object) { PageModel = new RequestAccountVerification.Model() };
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPost_GivenValidModelState_ExpectRedirectToPageResultToTheSamePage()
        {
            var mediator = new Mock<IMediator>();

            var page = new RequestAccountVerification(mediator.Object) { PageModel = new RequestAccountVerification.Model() };

            Assert.IsType<RedirectToPageResult>(await page.OnPostAsync());

            Assert.Equal(PrgState.Success, page.PrgState);
            mediator.Verify(
                x => x.Send(It.IsAny<RequestAccountVerificationCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        public class ValidatorTests
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var model = new RequestAccountVerification.Model { EmailAddress = "a@b.com" };
                var validator = new RequestAccountVerification.Validator();
                var result = validator.Validate(model);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenEmailAddressIsEmpty_ExpectValidationFailure()
            {
                var model = new RequestAccountVerification.Model { EmailAddress = string.Empty };
                var validator = new RequestAccountVerification.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
            }

            [Fact]
            public void Validate_GivenEmailAddressIsNotValidEmailAddress_ExpectValidationFailure()
            {
                var model = new RequestAccountVerification.Model { EmailAddress = "email-address" };
                var validator = new RequestAccountVerification.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
            }

            [Fact]
            public void Validate_GivenEmailAddressIsNull_ExpectValidationFailure()
            {
                var model = new RequestAccountVerification.Model { EmailAddress = null };
                var validator = new RequestAccountVerification.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
            }
        }
    }
}