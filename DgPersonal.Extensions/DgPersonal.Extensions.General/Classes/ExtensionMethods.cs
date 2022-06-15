using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace DgPersonal.Extensions.General.Classes
{
    public static class ExtensionMethods
    {
        public static bool HasValue(this string str) => string.IsNullOrWhiteSpace(str) == false;
        
        public static string StringToPascalCase(this string pascalCaseStr, bool usePluralName = false)
        {
            var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            var modelWithSpaces = r.Replace(pascalCaseStr, " ");
            var finalChar = pascalCaseStr[^1..];
            
            if (usePluralName)
                return finalChar == "s" ? $"{modelWithSpaces}es" : $"{modelWithSpaces}s";

            return modelWithSpaces;
        }
        
        public static string Capitalize(this string str)
        {
            var firstLetter = str[..1].ToUpper();
            var rest = str[1..];
            return $"{firstLetter}{rest}";
        }
        
        public static string Pluralize(this string str)
        {
            var final2 = str[^2..].ToLower();
            switch (final2)
            {
                case "ch":
                case "sh":
                    return $"{str}es";
                
                case "is":
                    return $"{str[..^2]}es";
            }

            switch (final2[1..])
            {
                case "s":
                case "x":
                case "z":
                    return $"{str}es";
                
                case "y":
                    return $"{str[..^1]}ies";
            }

            return $"{str}s";
        }
        
        public static int ToInt(this Enum enumerator) => Convert.ToInt32(enumerator);

        public static bool HasItems<T>(this IEnumerable<T> source)
            => source.OrEmptyIfNull().Any();
        
        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
            => source ?? Enumerable.Empty<T>();

        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
            => source.Select((item, index) => (item, index));

        private static string GetValue<T>(T item, string propertyName)
        {
            var type = typeof(T);
            if (type.IsPrimitive)
                return item.ToString() ?? "";
            
            PropertyInfo property;
            string value;
            
            if (propertyName.Contains("."))
            {
                var split = propertyName.Split(".");
                var cls = item.GetType().GetProperty(split[0])?.GetValue(item);

                property = cls.GetType().GetProperty(split[1]);
                value = property?.GetValue(cls)?.ToString() ?? "";
            }
            else
            {
                property = item.GetType().GetProperty(propertyName);
                value = property?.GetValue(item)?.ToString() ?? "";
            }

            return value;
        }

        public static string CamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            
            if (str.Length == 1)
                return str.ToLower();
            
            var first = str[..1].ToLower();
            var rest = str[1..];
            return $"{first}{rest}";
        }
        
        public static string ToCommaSeperatedString<T>(this List<T> items, string propertyName = null)
        {
            var index = 0;
            var sb = new StringBuilder();
            
            foreach (var item in items)
            {
                var value = GetValue(item, propertyName);

                if (string.IsNullOrEmpty(value))
                    continue;

                var isPascalCase = value.Contains(" ") == false 
                                   && value.Count(char.IsUpper) > 1;

                if (isPascalCase)
                    value = value.StringToPascalCase();
                
                var isLast = items.Count -1 == index;
                sb.Append(value).Append(isLast ? "" : ", ");
                index++;
            }

            return sb.ToString();
        }
        
        public static T GetValueOrDefault<T>(this IEnumerable<KeyValuePair<string, string>> keyValuePairs, string key)
        {
            var value = keyValuePairs.FirstOrDefault(x => x.Key == key).Value;
            switch (value)
            {
                case null:
                    return default;
                
                case T str:
                    return str;
            }

            var obj = Convert.ChangeType(value, typeof(T));
            return (T) obj;
        }
    }
}