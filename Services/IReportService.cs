using System.Threading.Tasks;

namespace Barangay.Services
{
    public interface IReportService
    {
        Task<byte[]> GenerateReportAsync();
    }
}
