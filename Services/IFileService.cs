using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Barangay.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string subDirectory);
    }
}
