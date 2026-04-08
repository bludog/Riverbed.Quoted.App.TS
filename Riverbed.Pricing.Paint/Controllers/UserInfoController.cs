using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Shared.Data;

namespace Riverbed.Pricing.Paint.Controllers
{
    public class UserInfoController : Controller
    {
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
    }
}
