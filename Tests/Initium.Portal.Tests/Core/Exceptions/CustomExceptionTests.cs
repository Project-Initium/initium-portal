// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Exceptions;
using Xunit;

namespace Initium.Portal.Tests.Core.Exceptions
{
    public class CustomExceptionTests
    {
        [Fact]
        public void Constructor_GivenValidArguments_ExpectMessageToBeSet()
        {
            var ex = new CustomException("some-error-message");

            Assert.Equal("some-error-message", ex.Message);
        }

        [Fact]
        public void Constructor_GivenValidArguments_ExpectMessageAndInnerExceptionToBeSet()
        {
            var innerEx = new Exception();
            var ex = new CustomException("some-error-message", innerEx);

            Assert.Equal("some-error-message", ex.Message);
            Assert.Equal(innerEx, ex.InnerException);
        }
    }
}