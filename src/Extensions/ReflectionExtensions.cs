﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Egil.RazorComponents.Bootstrap.Base.CssClassValues;

namespace Egil.RazorComponents.Bootstrap.Extensions
{
    public static class ReflectionExtensions
    {
        public static PropertyInfo[] GetInstanceProperties(this Type sourceType)
        {
            return sourceType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        }

        public static IEnumerable<PropertyInfo> IsAssignableFrom(this IEnumerable<PropertyInfo> properties, Type assignableFromType)
        {
            foreach (var prop in properties)
            {
                if (assignableFromType.IsAssignableFrom(prop.PropertyType))
                    yield return prop;
            }
        }

        public static IEnumerable<PropertyInfo> RemoveCssClassExcluded(this IEnumerable<PropertyInfo> properties)
        {
            foreach (var prop in properties)
            {
                var hasExcludedAttribute = prop.GetCustomAttribute<CssClassExcludedAttribute>();
                if (hasExcludedAttribute is null) yield return prop;
            }
        }

        public static T GetValue<T>(this PropertyInfo property, object sourceObject, T defaultIfNull = default)
        {
            var value = property.GetValue(sourceObject);
            if (value is null) return defaultIfNull;
            return (T)value;
        }

        public static IEnumerable<T> GetValues<T>(this IEnumerable<PropertyInfo> properties, object sourceObject) where T : class
        {
            foreach (var propInfo in properties)
            {
                if (propInfo.GetValue(sourceObject) is T value)
                    yield return value;
            }
        }
    }
}