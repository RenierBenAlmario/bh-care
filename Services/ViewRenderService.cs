using System;
using System.Threading.Tasks;

namespace Barangay.Services
{
    public class ViewRenderService : IViewRenderService
    {
        public Task<string> RenderToStringAsync(string viewName, object model)
        {
            // This is a placeholder implementation.
            Console.WriteLine($"Warning: {nameof(ViewRenderService)} is not fully implemented.");
            return Task.FromResult(string.Empty);
        }
    }
}
