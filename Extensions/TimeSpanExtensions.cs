using System;
using System.Collections.Generic;
using System.Globalization;
using Barangay.Helpers;

namespace Barangay.Extensions
{
    public static class TimeSpanExtensions
    {
        private static readonly string[] TimeFormats = { @"hh\:mm", "HH:mm", "h:mm tt", "hh:mm tt" };
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        public static string Format(this TimeSpan timeSpan, string format)
        {
            try
            {
                return DateTime.Today.Add(timeSpan).ToString(format, Culture);
            }
            catch (FormatException)
            {
                return "Invalid time format";
            }
            catch (Exception)
            {
                return timeSpan.ToString();
            }
        }

        public static string ToFormattedString(this TimeSpan timeSpan)
        {
            try
            {
                return DateTimeHelper.FormatTime(timeSpan);
            }
            catch (Exception)
            {
                return "00:00";
            }
        }

        public static string ToStandardFormat(this TimeSpan timeSpan)
        {
            try
            {
                return timeSpan.ToString(@"hh\:mm", Culture);
            }
            catch (FormatException)
            {
                return "00:00";
            }
        }

        public static TimeSpan ParseTimeSpan(string timeString)
        {
            return DateTimeHelper.ParseTime(timeString);
        }

        public static TimeSpan? ToNullableTimeSpan(this string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
                return null;

            var result = DateTimeHelper.ParseTime(timeString);
            return result == TimeSpan.Zero ? null : result;
        }

        public static bool IsValidTimeFormat(this string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
                return false;

            return TimeSpan.TryParseExact(timeString, TimeFormats, Culture, TimeSpanStyles.None, out _);
        }

        public static string ToDisplayFormat(this TimeSpan timeSpan)
        {
            return DateTime.Today.Add(timeSpan).ToString("h:mm tt", Culture);
        }
        
        public static TimeSpan DefaultIfNull(this TimeSpan? timeSpan)
        {
            return timeSpan ?? TimeSpan.Zero;
        }
        
        public static TimeSpan ToTimeSpanOrDefault(this string timeString, TimeSpan defaultValue)
        {
            if (TimeSpan.TryParseExact(timeString, TimeFormats, Culture, out TimeSpan result))
                return result;
            return defaultValue;
        }
        
        public static bool EqualsTimeSpan(this string timeString, TimeSpan timeSpan)
        {
            if (string.IsNullOrEmpty(timeString))
                return false;

            if (TimeSpan.TryParseExact(timeString, TimeFormats, Culture, out TimeSpan parsedTime))
                return parsedTime == timeSpan;

            return false;
        }

        public static string CoalesceWithTimeSpan(this string timeString, TimeSpan defaultTimeSpan)
        {
            return string.IsNullOrEmpty(timeString) ? defaultTimeSpan.ToDisplayFormat() : timeString;
        }

        public static bool TryParseTimeSpan(string timeString, out TimeSpan result)
        {
            result = DateTimeHelper.ParseTime(timeString);
            return result != TimeSpan.Zero || timeString == "00:00";
        }

        public static TimeSpan ParseTimeSpanOrDefault(string timeString, TimeSpan defaultValue = default)
        {
            var result = DateTimeHelper.ParseTime(timeString);
            return result == TimeSpan.Zero ? defaultValue : result;
        }

        // Add explicit FormatTime extension method to match missing references
        public static string FormatTime(this TimeSpan timeSpan)
        {
            try 
            {
                return DateTimeHelper.FormatTime(timeSpan);
            }
            catch (Exception)
            {
                return "00:00";
            }
        }
        
        // Add safe parsing utility that won't throw exceptions
        public static TimeSpan SafeParse(string timeString)
        {
            try
            {
                return DateTimeHelper.ParseTime(timeString);
            }
            catch
            {
                return TimeSpan.Zero;
            }
        }
    }
}