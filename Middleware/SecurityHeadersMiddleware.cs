using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Barangay.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        { 
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // This is a placeholder implementation.
            // Add security headers here, for example:
            // context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            
            await _next(context);
        }
    }
}
