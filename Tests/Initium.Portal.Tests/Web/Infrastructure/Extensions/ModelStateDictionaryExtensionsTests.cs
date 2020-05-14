// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Web.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;

namespace Initium.Portal.Tests.Web.Infrastructure.Extensions
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
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3459:Unassigned members should be removed", Justification = "Used as part of unit testing")]
            public string Name { get; }
        }
    }
}