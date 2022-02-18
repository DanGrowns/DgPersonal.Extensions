using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace DgPersonal.Extensions.General.Classes
{
    public static class EnumHelperExtensions 
    {
        public static IList<T> GetValues<T>(this Enum value) where T : struct, Enum
            => EnumHelper<T>.GetValues(value);

        public static T Parse<T>(string value) where T : struct, Enum
            => EnumHelper<T>.Parse(value);

        public static IList<string> GetNames<T>(this Enum value) where T : struct, Enum
            => EnumHelper<T>.GetNames(value);

        public static IList<string> GetDisplayValues<T>(this Enum value) where T : struct, Enum
            => EnumHelper<T>.GetDisplayValues(value);

        public static string GetDisplayValue<T>(this T value) where T : struct, Enum
            => EnumHelper<T>.GetDisplayValue(value);
        
        public static string GetDescriptionValue<T>(this T value) where T : struct, Enum
            => EnumHelper<T>.GetDescriptionValue(value);
    }
    
    public static class EnumHelper<T> where T : struct, Enum
    {
        private static string LookupResource(Type resourceManagerProvider, string resourceKey)
        {
            var resourceKeyProperty = resourceManagerProvider.GetProperty(resourceKey,
                BindingFlags.Static | BindingFlags.Public, null, typeof(string),
                new Type[0], null);
            
            if (resourceKeyProperty != null)
                return (string) resourceKeyProperty.GetMethod.Invoke(null, null);
            
            return resourceKey; // Fallback with the key name
        }
        
        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            
            return enumValues;
        }

        public static T Parse(string value)
            => (T)Enum.Parse(typeof(T), value, true);
        
        public static IList<string> GetNames(Enum value)
            => value.GetType()
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Select(fi => fi.Name)
                .ToList();
        
        public static IList<string> GetDisplayValues(Enum value)
            => GetNames(value)
                .Select(obj => GetDisplayValue(Parse(obj)))
                .ToList();
        
        public static string GetDisplayValue(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo?.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes?.Length == 0)
                return value.ToString();
             
            if (descriptionAttributes[0].ResourceType != null)
                return LookupResource(descriptionAttributes[0].ResourceType, descriptionAttributes[0].Name);

            return descriptionAttributes[0].Name;
        }
        
        public static string GetDescriptionValue(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo?.GetCustomAttributes(
                typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            return descriptionAttributes?.Length > 0 
                ? descriptionAttributes[0].Description 
                : value.ToString();
        }
        
        public static IEnumerable<BindingData> AsTextValuePair()
        {
            var list = new List<BindingData>();
            
            foreach (var value in Enum.GetValues<T>())
            {
                list.Add(new BindingData(value.GetDisplayValue(), value.ToString()));
            }

            return list.ToArray();
        }
    }
}