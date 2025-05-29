# Date Migration Guide

This document provides guidance on fixing date-related errors in the codebase. The application handles appointment dates as strings in "yyyy-MM-dd" format, which requires careful handling when comparing or converting dates.

## Common Errors and Solutions

### 1. Method group errors

**Error**: `Operator '==' cannot be applied to operands of type 'method group' and 'DateTime'`

This happens when you try to compare the result of a `Date()` extension method to a DateTime.

❌ **Incorrect**:
```csharp
if (appointment.AppointmentDate.Date == DateTime.Today)
```

✅ **Correct**:
```csharp
// Call the Date() method explicitly
if (appointment.AppointmentDate.Date() == DateTime.Today)

// Or use IsEqual method from extensions
if (appointment.AppointmentDate.IsEqual(DateTime.Today))
```

### 2. Type conversion errors

**Error**: `Cannot implicitly convert type 'System.DateTime' to 'string'`

This happens when assigning a DateTime to a string property.

❌ **Incorrect**:
```csharp
appointment.AppointmentDate = DateTime.Today;
```

✅ **Correct**:
```csharp
// Use ToDateString extension method
appointment.AppointmentDate = DateTime.Today.ToDateString();
```

### 3. Date comparison errors

**Error**: `Operator '>=' cannot be applied to operands of type 'string' and 'DateTime'`

This happens when comparing a string date with a DateTime.

❌ **Incorrect**:
```csharp
if (appointment.AppointmentDate >= DateTime.Today)
```

✅ **Correct**:
```csharp
// Use IsGreaterThanOrEqual extension method
if (appointment.AppointmentDate.IsGreaterThanOrEqual(DateTime.Today))

// Or use string comparison if both are strings
if (string.Compare(appointment.AppointmentDate, DateTime.Today.ToDateString()) >= 0)
```

### 4. Format provider errors

**Error**: `Argument 1: cannot convert from 'string' to 'System.IFormatProvider?'`

This happens in string formatting operations.

❌ **Incorrect**:
```csharp
string formattedDate = appointment.AppointmentDate.ToString("MM/dd/yyyy", "en-US");
```

✅ **Correct**:
```csharp
// Use FormatDate extension method
string formattedDate = appointment.AppointmentDate.FormatDate("MM/dd/yyyy", CultureInfo.GetCultureInfo("en-US"));

// Or format directly if converting to DateTime first
string formattedDate = appointment.AppointmentDate.ToDateTime().ToString("MM/dd/yyyy", CultureInfo.GetCultureInfo("en-US"));
```

## Examples of Complete Fixes

### Example 1: Appointment validation

```csharp
// Before
if (appointment.AppointmentDate.Date == DateTime.Today && appointment.AppointmentTime <= DateTime.Now.TimeOfDay)
{
    return BadRequest("For today's appointments, please select a future time.");
}

// After
string todayString = DateTime.Today.ToDateString();
bool isToday = string.Equals(appointment.AppointmentDate, todayString, StringComparison.Ordinal);
if (isToday && appointment.AppointmentTime <= DateTime.Now.TimeOfDay)
{
    return BadRequest("For today's appointments, please select a future time.");
}
```

### Example 2: Date range filtering

```csharp
// Before
var appointments = await _context.Appointments
    .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
    .ToListAsync();

// After
string startDateString = startDate.ToDateString();
string endDateString = endDate.ToDateString();

var appointments = await _context.Appointments
    .Where(a => string.Compare(a.AppointmentDate, startDateString) >= 0 && 
                string.Compare(a.AppointmentDate, endDateString) <= 0)
    .ToListAsync();

// Or using extension methods
var appointments = await _context.Appointments
    .Where(a => a.AppointmentDate.IsGreaterThanOrEqual(startDate) && 
                a.AppointmentDate.IsLessThanOrEqual(endDate))
    .ToListAsync();
```

## Available Extension Methods

### String Date Extensions (DateExtensions.cs)

- `ToDateTime(this string dateString)`: Converts a date string to DateTime
- `Date(this string dateString)`: Gets the Date property from a date string
- `IsGreaterThanOrEqual(this string dateString, DateTime otherDate)`: Checks if date string is >= DateTime
- `IsLessThanOrEqual(this string dateString, DateTime otherDate)`: Checks if date string is <= DateTime
- `IsLessThan(this string dateString, DateTime otherDate)`: Checks if date string is < DateTime
- `IsEqual(this string dateString, DateTime otherDate)`: Checks if date string equals DateTime date
- `IsEqual(this string dateString, string otherDateString)`: Checks if two date strings are equal
- `FormatDate(this string dateString, string format, IFormatProvider formatProvider = null)`: Formats a date string
- `ToDateString(this DateTime date)`: Converts a DateTime to a date string in "yyyy-MM-dd" format
- `ToNullableDateTime(this string dateString)`: Converts a date string to a nullable DateTime
- `FormatWithCulture(this string dateString, string format, string cultureName)`: Formats a date string with a culture name
- `AddDays(this string dateString, int days)`: Adds days to a date string

### DateTime Extensions (DateTimeExtensions.cs)

- `Date(this DateTime date)`: Gets the Date property from a DateTime (for compatibility with string.Date())
- `IsEqual(this DateTime date, string dateString)`: Checks if DateTime equals date string
- `IsGreaterThan(this DateTime date, string dateString)`: Checks if DateTime > date string
- `IsLessThan(this DateTime date, string dateString)`: Checks if DateTime < date string
- `IsGreaterThanOrEqual(this DateTime date, string dateString)`: Checks if DateTime >= date string
- `IsLessThanOrEqual(this DateTime date, string dateString)`: Checks if DateTime <= date string

### Format Provider Extensions (FormatProviderExtensions.cs)

- `GetProvider(string cultureName)`: Gets an IFormatProvider from a culture name
- `ToString(this DateTime date, string format, string cultureName)`: Formats a DateTime with a culture name
- `ToFormattedString(this DateTime date, string format)`: Formats a DateTime with invariant culture
- `ToDisplayDate(this DateTime date)`: Formats a DateTime as "MM/dd/yyyy"
- `ToDisplayDateTime(this DateTime date)`: Formats a DateTime as "MM/dd/yyyy hh:mm tt" 