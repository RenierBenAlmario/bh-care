using System.Threading.Tasks;
using System.Collections.Generic;
using Barangay.Models;
using Microsoft.AspNetCore.Identity;

namespace Barangay.Services
{
    public interface IUserVerificationService
    {
        Task<List<ApplicationUser>> GetPendingUsersAsync();
        Task<bool> ApproveUserAsync(string userId, string adminId);
        Task<bool> RejectUserAsync(string userId, string adminId, string reason = null);
        Task<bool> IsUserVerifiedAsync(string userId);
        Task<string> GetUserStatusAsync(string userId);
        Task EnsureUserRoleAssignedAsync(string userId);
    }
} 