// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace Initium.Portal.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplay(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            if (name == null)
            {
                return string.Empty;
            }

            var field = type.GetField(name);
            if (Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attr)
            {
                return attr.Name;
            }

            return string.Empty;
        }
    }
}