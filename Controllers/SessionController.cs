using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;

namespace Barangay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SessionController : ControllerBase
    {
        [HttpPost("KeepAlive")]
        public IActionResult KeepAlive()
        {
            // This endpoint simply keeps the session alive by making a request
            // The act of making an authenticated request refreshes the session
            
            // Update last activity timestamp
            HttpContext.Session.SetString("LastActivity", DateTime.UtcNow.ToString("o"));
            
            return Ok(new { success = true, message = "Session refreshed" });
        }
    }
} 