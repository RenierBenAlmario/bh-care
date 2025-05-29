using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;

namespace Barangay.Extensions
{
    public static class TempDataExtensions
    {
        public static void Set<T>(this ITempDataDictionary tempData, string key, T value)
        {
            tempData[key] = JsonSerializer.Serialize(value);
        }

        public static T? Get<T>(this ITempDataDictionary tempData, string key)
        {
            if (!tempData.TryGetValue(key, out object? value) || value == null)
                return default;

            return value is string stringValue 
                ? JsonSerializer.Deserialize<T>(stringValue) 
                : default;
        }
    }
} 