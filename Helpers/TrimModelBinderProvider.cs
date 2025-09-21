using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Barangay.Helpers
{
    public class TrimModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            // This is a placeholder implementation.
            return null;
        }
    }
}
