# OTP Console Display Guide

## Overview
The BHCARE application now supports displaying OTP (One-Time Password) codes on the console during development and deployment. This feature is particularly useful for testing and debugging purposes when email delivery might not be available or reliable.

## Features

### Console Display
When an OTP is generated, it will be displayed in a visually distinct format on the console:

```
╔══════════════════════════════════════════════════════════════════════════════╗
║                              OTP VERIFICATION CODE                          ║
╠══════════════════════════════════════════════════════════════════════════════╣
║  Email: doctor@example.com                                                  ║
║  OTP Code: 123456                                                           ║
║  Generated: 2024-01-15 14:30:25 UTC                                         ║
║  Expires: 2024-01-15 14:35:25 UTC                                           ║
╚══════════════════════════════════════════════════════════════════════════════╝
```

### Configuration
The console display can be controlled via the `OTPSettings` section in your configuration files:

```json
{
  "OTPSettings": {
    "EnableConsoleDisplay": true,
    "ConsoleDisplayDescription": "OTP codes are displayed on console for development and deployment testing"
  }
}
```

## Configuration Files

### Development (appsettings.Development.json)
```json
{
  "OTPSettings": {
    "EnableConsoleDisplay": true,
    "ConsoleDisplayDescription": "OTP codes are displayed on console for development testing"
  }
}
```

### Production (appsettings.Production.json)
```json
{
  "OTPSettings": {
    "EnableConsoleDisplay": true,
    "ConsoleDisplayDescription": "OTP codes are displayed on console for deployment testing"
  }
}
```

## How It Works

1. **OTP Generation**: When a user requires OTP verification (Gmail accounts, specific test accounts), the system generates a 6-digit OTP.

2. **Console Display**: If `EnableConsoleDisplay` is set to `true`, the OTP is displayed in a formatted box on the console.

3. **Email Sending**: The OTP is still sent via email as before, but now you also have console access for immediate testing.

4. **Logging**: The OTP information is also logged using structured logging for production monitoring.

## Usage Scenarios

### Development Testing
- Enable console display in development environment
- Use console OTP for quick testing without waiting for emails
- Debug OTP generation and validation flows

### Deployment Testing
- Enable console display during initial deployment
- Verify OTP functionality works correctly
- Test user authentication flows

### Production Monitoring
- Console display can be disabled in production for security
- Structured logging still captures OTP events for monitoring
- Email delivery remains the primary OTP method

## Security Considerations

### Development Environment
- Console display is safe for development
- Helps with testing and debugging
- No security concerns in local development

### Production Environment
- Console display can be disabled by setting `EnableConsoleDisplay: false`
- OTP codes are still logged for monitoring purposes
- Email delivery remains the primary authentication method

## Disabling Console Display

To disable console display, update your configuration:

```json
{
  "OTPSettings": {
    "EnableConsoleDisplay": false
  }
}
```

## Testing the Feature

1. **Start the application**:
   ```bash
   dotnet run
   ```

2. **Attempt to login** with a Gmail account or test account that requires OTP

3. **Check the console output** - you should see the formatted OTP display

4. **Use the displayed OTP** to complete the login process

## Troubleshooting

### Console Display Not Showing
- Check that `EnableConsoleDisplay` is set to `true` in your configuration
- Verify the configuration file is being loaded correctly
- Check application logs for any errors

### OTP Not Working
- Verify the OTP code from console matches what's expected
- Check OTP expiration time (5 minutes from generation)
- Ensure the email address matches exactly

## Integration with Existing Features

This feature integrates seamlessly with the existing OTP system:

- **Email Service**: OTPs are still sent via email
- **Caching**: OTPs are still cached for validation
- **Expiration**: 5-minute expiration remains unchanged
- **Validation**: Same validation logic applies

## Environment Variables

For Azure deployment, you can also control this via environment variables:

```bash
OTPSettings__EnableConsoleDisplay=true
```

This allows you to enable/disable console display without modifying configuration files during deployment.
