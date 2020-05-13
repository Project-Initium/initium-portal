// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Web.Controllers.Api.User.Models;
using Xunit;

namespace Stance.Tests.Web.Controllers.Api.User.Models
{
    public class UnlockAccountRequestValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var request = new UnlockAccountRequest
            {
                UserId = TestVariables.UserId,
            };
            var validator = new UnlockAccountRequest.Validator();
            var result = validator.Validate(request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenNameIsEmpty_ExpectValidationFailure()
        {
            var request = new UnlockAccountRequest
            {
                UserId = Guid.Empty,
            };
            var validator = new UnlockAccountRequest.Validator();
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.PropertyName == "UserId");
        }
    }
}