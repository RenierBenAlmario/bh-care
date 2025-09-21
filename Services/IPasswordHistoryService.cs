using System.Threading.Tasks;
using Barangay.Models;

namespace Barangay.Services
{
    public interface IPasswordHistoryService
    {
        Task AddPasswordHistoryAsync(ApplicationUser user, string passwordHash);
        Task<bool> IsPasswordInHistoryAsync(string userId, string passwordHash);
    }
}
