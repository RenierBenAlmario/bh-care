using System;
using System.Globalization;
using Barangay.Helpers;

namespace Barangay.Extensions
{
    /// <summary>
    /// Extension methods for converting between different date types and handling special cases.
    /// </summary>
    public static class DateConversionExtensions
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        // Special case for mapping model properties
        public static string MapDateFromModel(this object model, string propertyName)
        {
            var property = model.GetType().GetProperty(propertyName);
            if (property == null) return string.Empty;
            
            var value = property.GetValue(model);
            if (value == null) return string.Empty;
            
            if (value is DateTime dt)
                return DateTimeHelper.FormatDate(dt);
                
            return value.ToString();
        }
        
        // Convert any object to DateTime safely
        public static DateTime ToDateTimeSafe(this object value)
        {
            if (value == null) return DateTime.MinValue;
            
            if (value is DateTime dt) return dt;
            
            if (value is string str) return DateTimeHelper.ParseDate(str);
            
            return DateTime.MinValue;
        }
        
        // Convert any object to nullable DateTime safely
        public static DateTime? ToNullableDateTimeSafe(this object value)
        {
            if (value == null) return null;
            
            if (value is DateTime dt) return dt;
            
            if (value is string str) return DateTimeHelper.ParseNullableDate(str);
            
            return null;
        }
        
        // Handle common format provider cases
        public static string FormatWithCulture(this object value, string format, string cultureName)
        {
            return value.ToDateTimeSafe().ToString(format, FormatProviderExtensions.GetProvider(cultureName));
        }
        
        // Convert medical record dates
        public static DateTime GetRecordDate(this object record)
        {
            if (record == null) return DateTime.MinValue;
            
            var property = record.GetType().GetProperty("RecordDate");
            if (property == null) return DateTime.MinValue;
            
            var value = property.GetValue(record);
            return value.ToDateTimeSafe();
        }
        
        // Special case for AppointmentCreateModel.Date
        public static DateTime GetModelDate(this object model)
        {
            if (model == null) return DateTime.MinValue;
            
            var property = model.GetType().GetProperty("Date");
            if (property == null) return DateTime.MinValue;
            
            var value = property.GetValue(model);
            return value.ToDateTimeSafe();
        }
        
        // Check if a date is within a range
        public static bool IsInDateRange(this string dateString, DateTime? startDate, DateTime? endDate)
        {
            var date = DateTimeHelper.ParseDate(dateString);
            
            if (startDate.HasValue && date < startDate.Value.Date)
                return false;
                
            if (endDate.HasValue && date > endDate.Value.Date)
                return false;
                
            return true;
        }

        public static DateTime ToDateTime(this string dateString)
        {
            return DateTimeHelper.ParseDate(dateString);
        }

        public static DateTime? ToNullableDateTime(this string dateString)
        {
            return DateTimeHelper.ParseNullableDate(dateString);
        }

        public static string ToDateString(this DateTime date)
        {
            return DateTimeHelper.FormatDate(date);
        }

        public static string ToTimeString(this DateTime date)
        {
            return date.ToString("h:mm tt", Culture);
        }

        public static string ToDateTimeString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss", Culture);
        }

        public static bool IsValidDate(this string dateString)
        {
            return DateTimeHelper.ParseDate(dateString) != DateTime.MinValue;
        }
    }
} 