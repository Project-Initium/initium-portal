// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Initium.Portal.Web.Infrastructure.Extensions
{
    public static class DictionaryExtensions
    {
        public static T ToObject<T>(this IDictionary<string, object> source)
            where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var (key, value) in source)
            {
                var prop = someObjectType.GetProperty(key);
                if (prop != null)
                {
                    prop.SetValue(someObject, value, null);
                }
            }

            return someObject;
        }
    }
}