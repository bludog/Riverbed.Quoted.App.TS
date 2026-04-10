using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Shared.Data;
using System.Security.Claims;

namespace Riverbed.Pricing.Paint.Controllers
{
    public class UserInfoController : Controller
    {
        public sealed class CurrentUserResponse
        {
            public required string UserId { get; set; }
            public required string Email { get; set; }
            public List<string> Roles { get; set; } = [];
            public bool IsDarkMode { get; set; }
        }

        private readonly ApplicationDbContext _context;

        public UserInfoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("api/UserInfo/Company/{email}")]
        public IActionResult GetUserCompanyGuid(string email)
        {
            var userCompanyGuid = _context.Users.FirstOrDefault(u => u.Email == email).CompanyGuid;
            return Ok(userCompanyGuid);
        }

        [HttpGet("api/UserInfo/Role/{email}")]
        public IActionResult GetUserRole(string email)
        { 
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id);
            if (userRole == null)
            {
                return NotFound();
            }
            var role = _context.Roles.FirstOrDefault(r => r.Id == userRole.RoleId);
            return Ok(role.Name);
        }

        //Get AspNetUser id by email
        [HttpGet("api/UserInfo/UserId/{email}")]
        public IActionResult GetUserId(string email)
        {
            var userId = _context.Users.FirstOrDefault(u => u.Email == email).Id;
            return Ok(userId);
        }

        /// <summary>
        /// Gets the dark mode preference for the user identified by their ASP.NET Identity ID.
        /// </summary>
        [HttpGet("api/UserInfo/DarkMode/{userId}")]
        public async Task<IActionResult> GetDarkMode(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return NotFound();

            return Ok(user.IsDarkMode);
        }

        /// <summary>
        /// Sets the dark mode preference for the user identified by their ASP.NET Identity ID.
        /// </summary>
        [HttpPut("api/UserInfo/DarkMode/{userId}")]
        public async Task<IActionResult> SetDarkMode(string userId, [FromBody] bool isDarkMode)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
                return NotFound();

            user.IsDarkMode = isDarkMode;
            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Gets the current authenticated user profile for WebAssembly authentication state.
        /// </summary>
        [Authorize]
        [HttpGet("api/UserInfo/Current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null || string.IsNullOrWhiteSpace(user.Email))
            {
                return Unauthorized();
            }

            var roles = User.FindAll(ClaimTypes.Role).Select(role => role.Value).Distinct().ToList();

            return Ok(new CurrentUserResponse
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = roles,
                IsDarkMode = user.IsDarkMode
            });
        }
    }
}
