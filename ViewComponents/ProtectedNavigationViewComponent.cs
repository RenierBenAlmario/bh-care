using Barangay.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Barangay.ViewComponents
{
    public class ProtectedNavigationViewComponent : ViewComponent
    {
        private readonly IProtectedUrlService _protectedUrlService;
        private readonly ILogger<ProtectedNavigationViewComponent> _logger;

        public ProtectedNavigationViewComponent(IProtectedUrlService protectedUrlService, ILogger<ProtectedNavigationViewComponent> logger)
        {
            _protectedUrlService = protectedUrlService;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new ProtectedNavigationViewModel();

            if (User.Identity?.IsAuthenticated == true)
            {
                var claimsPrincipal = User as ClaimsPrincipal;
                var userId = claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    var resourceType = DetermineResourceType(claimsPrincipal);

                    try
                    {
                        // Generate protected URLs based on user role
                        if (User.IsInRole("Doctor") || User.IsInRole("Admin"))
                        {
                            model.DoctorPatientListUrl = await _protectedUrlService.GenerateProtectedPageUrlAsync(
                                resourceType, userId, "/Doctor/PatientList");
                            model.DoctorDashboardUrl = await _protectedUrlService.GenerateProtectedPageUrlAsync(
                                resourceType, userId, "/Doctor/Dashboard");
                        }

                        if (User.IsInRole("Nurse") || User.IsInRole("Admin"))
                        {
                            model.NurseDashboardUrl = await _protectedUrlService.GenerateProtectedPageUrlAsync(
                                resourceType, userId, "/Nurse/Dashboard");
                            model.NursePatientQueueUrl = await _protectedUrlService.GenerateProtectedPageUrlAsync(
                                resourceType, userId, "/Nurse/PatientQueue");
                        }

                        if (User.IsInRole("Admin") || User.IsInRole("System Administrator"))
                        {
                            model.AdminDashboardUrl = await _protectedUrlService.GenerateProtectedPageUrlAsync(
                                resourceType, userId, "/Admin/Dashboard");
                            model.AdminUserManagementUrl = await _protectedUrlService.GenerateProtectedPageUrlAsync(
                                resourceType, userId, "/Admin/UserManagement");
                        }

                        if (User.IsInRole("User"))
                        {
                            model.UserDashboardUrl = await _protectedUrlService.GenerateProtectedPageUrlAsync(
                                resourceType, userId, "/User/UserDashboard");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error generating protected URLs for user {UserId}", userId);
                    }
                }
            }

            return View(model);
        }

        private static string DetermineResourceType(ClaimsPrincipal user)
        {
            if (user.IsInRole("Admin") || user.IsInRole("System Administrator"))
                return "Admin";
            if (user.IsInRole("Doctor"))
                return "Doctor";
            if (user.IsInRole("Nurse") || user.IsInRole("Head Nurse"))
                return "Nurse";
            if (user.IsInRole("User"))
                return "User";
            
            return "User"; // Default fallback
        }
    }

    public class ProtectedNavigationViewModel
    {
        public string? DoctorPatientListUrl { get; set; }
        public string? DoctorDashboardUrl { get; set; }
        public string? NurseDashboardUrl { get; set; }
        public string? NursePatientQueueUrl { get; set; }
        public string? AdminDashboardUrl { get; set; }
        public string? AdminUserManagementUrl { get; set; }
        public string? UserDashboardUrl { get; set; }
    }
}
