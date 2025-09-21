using System.Threading.Tasks;
using Barangay.Models;

namespace Barangay.Services
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserByIdAsync(string userId);
    }
}
