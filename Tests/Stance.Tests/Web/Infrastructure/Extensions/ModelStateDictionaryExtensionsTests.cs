// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Stance.Web.Infrastructure.Extensions;
using Xunit;

namespace Stance.Tests.Web.Infrastructure.Extensions
{
    public class ModelStateDictionaryExtensionsTests
    {
        [Fact]
        public void ToModelState_GivenValidJsonString_ExpectModelState()
        {
            var modelStateDictionary =
                "[{\"Key\":\"Name\",\"AttemptedValue\":null,\"RawValue\":null,\"ErrorMessages\":[\"Field is required\"]}]"
                    .ToModelState();

            Assert.Equal(1, modelStateDictionary.ErrorCount);
            Assert.Contains(modelStateDictionary, x => x.Key == "Name");
        }

        [Fact]
        public void ToSerializedString_GivenValidModelState_ExpectJsonString()
        {
            var modelStateDictionary = new ModelStateDictionary();
            modelStateDictionary.AddModelError<ModelStateChecker>(x => x.Name, "Field is required");

            var result = modelStateDictionary.ToSerializedString();

            Assert.Equal(
                "[{\"Key\":\"Name\",\"AttemptedValue\":null,\"RawValue\":null,\"ErrorMessages\":[\"Field is required\"]}]",
                result);
        }

        private class ModelStateChecker
        {
            public string Name { get; set; }
        }
    }
}