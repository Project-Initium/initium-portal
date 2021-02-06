// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;

namespace Initium.Portal.Tests
{
    public static class Helpers
    {
        internal static TEntity CreateEntity<TEntity>(object props)
        {
            var type = typeof(TEntity);
            var entity = Activator.CreateInstance(type, true);

            foreach (var prop in props.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (prop.PropertyType.Name == "List`1")
                {
                    type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                        .Single(x => x.Name.ToLower().Contains(prop.Name.ToLower()))
                        .SetValue(entity, prop.GetValue(props));
                }
                else
                {
                    type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Single(propertyInfo => string.Equals(propertyInfo.Name, prop.Name,
                            StringComparison.CurrentCultureIgnoreCase))
                        .SetValue(entity, prop.GetValue(props));
                }
            }

            return (TEntity)entity;
        }
    }
}