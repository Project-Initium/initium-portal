// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Stance.Web.Infrastructure.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static string ToSerializedString(this ModelStateDictionary modelState)
        {
            var errorList = modelState
                .Select(kvp => new ModelStateTransferContainer(
                    kvp.Key,
                    kvp.Value.AttemptedValue,
                    kvp.Value.RawValue,
                    kvp.Value.Errors.Select(err => err.ErrorMessage).ToList()));

            return JsonConvert.SerializeObject(errorList, Formatting.None);
        }

        public static ModelStateDictionary ToModelState(this string serializedModelState)
        {
            var errorList = JsonConvert.DeserializeObject<List<ModelStateTransferContainer>>(serializedModelState);
            var modelState = new ModelStateDictionary();

            foreach (var item in errorList)
            {
                modelState.SetModelValue(item.Key, item.RawValue, item.AttemptedValue);
                foreach (var error in item.ErrorMessages)
                {
                    modelState.AddModelError(item.Key, error);
                }
            }

            return modelState;
        }

        public class ModelStateTransferContainer
        {
            public ModelStateTransferContainer(string key, string attemptedValue, object rawValue,
                ICollection<string> errorMessages)
            {
                this.Key = key;
                this.AttemptedValue = attemptedValue;
                this.RawValue = rawValue;
                this.ErrorMessages = errorMessages;
            }

            public string Key { get; }

            public string AttemptedValue { get; }

            public object RawValue { get; }

            public ICollection<string> ErrorMessages { get; }
        }
    }
}