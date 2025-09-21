using System.Security.Claims;

namespace Barangay.Extensions
{
    public static class UserExtensions
    {
        /// <summary>
        /// Gets the user ID from the ClaimsPrincipal
        /// </summary>
        /// <param name="user">The ClaimsPrincipal representing the current user</param>
        /// <returns>The user ID or null if not found</returns>
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
} 