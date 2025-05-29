using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace Barangay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IConfiguration _configuration;

        public AuthenticationController(ILogger<AuthenticationController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // Dummy user database
        private readonly Dictionary<string, string> users = new Dictionary<string, string>
        {
            { "admin", "D033E22AE348AEB5660FC2140AEC35850C4DA997" } // SHA-1 for "password123"
        };

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] UserModel user)
        {
            if (users.ContainsKey(user.Username) && VerifyPassword(user.Password, users[user.Username]))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("your_secret_key");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Username) }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new { token = tokenString });
            }
            else
            {
                _logger.LogWarning($"Invalid login attempt for user: {user.Username}");
                return Unauthorized(new { message = "Invalid credentials" });
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] enteredBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
                StringBuilder enteredHash = new StringBuilder();
                foreach (byte b in enteredBytes)
                {
                    enteredHash.Append(b.ToString("x2"));
                }
                return enteredHash.ToString().ToUpper() == storedHash;
            }
        }
    }

    public class UserModel
    {
      public string Username { get; set; } = ""; // Initialize with an empty string or a default value
        public string Password { get; set; } = ""; // Initialize with an empty string or a default value
    }
}
