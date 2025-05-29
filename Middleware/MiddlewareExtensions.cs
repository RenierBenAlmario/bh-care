using Microsoft.AspNetCore.Builder;

namespace Barangay.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseVerifiedUserMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<VerifiedUserMiddleware>();
        }
    }
} 