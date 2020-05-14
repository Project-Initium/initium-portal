// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Web.Controllers.Api.AuthApp.Models;
using Xunit;

namespace Initium.Portal.Tests.Web.Controllers.Api.AuthApp.Models
{
    public class RevokeAuthAppRequestValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var request = new RevokeAuthAppRequest
            {
                Password = "password",
            };
            var validator = new RevokeAuthAppRequest.Validator();
            var result = validator.Validate(request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenPasswordIsEmpty_ExpectValidationFailure()
        {
            var request = new RevokeAuthAppRequest
            {
                Password = string.Empty,
            };
            var validator = new RevokeAuthAppRequest.Validator();
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.PropertyName == "Password");
        }

        [Fact]
        public void Validate_GivenPasswordIsNull_ExpectValidationFailure()
        {
            var request = new RevokeAuthAppRequest
            {
                Password = null,
            };
            var validator = new RevokeAuthAppRequest.Validator();
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.PropertyName == "Password");
        }
    }
}