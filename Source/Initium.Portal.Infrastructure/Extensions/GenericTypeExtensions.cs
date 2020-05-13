// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;

namespace Initium.Portal.Infrastructure.Extensions
{
    internal static class GenericTypeExtensions
    {
        public static string GetGenericTypeName(this object @object)
        {
            return @object.GetType().GetGenericTypeName();
        }

        private static string GetGenericTypeName(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
            return $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
    }
}