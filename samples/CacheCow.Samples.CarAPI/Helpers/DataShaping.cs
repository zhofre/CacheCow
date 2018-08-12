using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CacheCow.Samples.CarAPI.Helpers
{
    public static class DataShaping
    {
        public static ExpandoObject Shape<T>(this T source, List<string> fields)
        {
            if (source == null)
            {
                return null;
            }

            var result = new ExpandoObject();

            var propertiesToAdd = fields == null || fields.Count == 0
                ? typeof(T).GetProperties((BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance))
                : typeof(T).GetProperties((BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance))
                    .Where(p => fields.Any(f => string.Compare(f, p.Name, true) == 0))
                    .ToArray();
            var resultDictionary = (IDictionary<string, object>)result;
            foreach (var prop in propertiesToAdd)
            {
                var value = prop.GetValue(source);
                resultDictionary.Add(prop.Name, value);
            }

            return result;
        }

        public static ExpandoObject ShapeCollection<T>(this Dto.LinkedResourceCollection<T> source, List<string> fields)
            where T : Dto.LinkedResource
        {
            var shapedContent = source.Content
                .Select(x => x.Shape(fields))
                .ToList();

            dynamic result = new ExpandoObject();
            result.Content = shapedContent;
            result.Links = source.Links;

            return result;
        }

        public static List<string> ParseFieldsFromQueryString(this string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return null;
            }

            var unescapedFields = Uri.UnescapeDataString(fields);
            var purgedFields = new string(unescapedFields
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
            return purgedFields.Split(',')
                .ToList();
        }
    }
}
