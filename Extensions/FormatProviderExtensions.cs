using System;
using System.Globalization;

namespace Barangay.Extensions
{
    public static class FormatProviderExtensions
    {
        // Get a format provider from a culture name
        public static IFormatProvider GetProvider(string cultureName)
        {
            try
            {
                return CultureInfo.GetCultureInfo(cultureName);
            }
            catch
            {
                return CultureInfo.InvariantCulture;
            }
        }
        
        // Extension method for DateTime.ToString with culture name
        public static string ToString(this DateTime date, string format, string cultureName)
        {
            return date.ToString(format, GetProvider(cultureName));
        }
        
        // Extension method for formatting DateTime to specific format with invariant culture
        public static string ToFormattedString(this DateTime date, string format)
        {
            return date.ToString(format, CultureInfo.InvariantCulture);
        }
        
        // Extension method for formatted date display
        public static string ToDisplayDate(this DateTime date)
        {
            return date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
        }
        
        // Extension method for formatted date and time display
        public static string ToDisplayDateTime(this DateTime date)
        {
            return date.ToString("MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
        }
    }
} 