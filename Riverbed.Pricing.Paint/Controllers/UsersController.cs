using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator, CompanyAdmin")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly PricingDbContext _context;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        PricingDbContext context,
        ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _logger = logger;
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        try
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Id == user.CompanyGuid);

                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    CompanyGuid = user.CompanyGuid,
                    CompanyName = company?.CompanyName ?? "Not Assigned",
                    IsLockedOut = user.LockoutEnabled,
                    Roles = roles.ToList()
                });
            }

            return Ok(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, "Internal server error while retrieving users");
        }
    }

    // GET: api/users/roles
    [HttpGet("roles")]
    public async Task<ActionResult<List<string>>> GetAvailableRoles()
    {
        try
        {
            var roles = await _roleManager.Roles
                .Select(r => r.Name)
                .Where(n => n != null)
                .ToListAsync();
            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles");
            return StatusCode(500, "Internal server error while retrieving roles");
        }
    }

    // Get User Roles using email address
    [HttpGet("roles/{email}")]
    public async Task<ActionResult<List<string>>> GetUserRolesByEmail(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound($"User with email {email} not found");
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles for user with email {Email}", email);
            return StatusCode(500, "Internal server error while retrieving user roles");
        }
    }

    // GET: api/users/companies
    [HttpGet("companies")]
    public async Task<ActionResult<List<CompanyDto>>> GetCompanies()
    {
        try
        {
            var companies = await _context.Companies
                .Select(c => new CompanyDto
                {
                    Id = c.Id.ToString(),
                    Name = c.CompanyName ?? string.Empty
                })
                .ToListAsync();
            return Ok(companies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving companies");
            return StatusCode(500, "Internal server error while retrieving companies");
        }
    }

    // PUT: api/users/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UserDto userDto)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound($"User with ID {id} not found");

            user.UserName = userDto.UserName;
            user.Email = userDto.Email;
            user.LockoutEnabled = userDto.IsLockedOut;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return StatusCode(500, "Internal server error while updating user");
        }
    }

    // PUT: api/users/{id}/roles
    [HttpPut("{id}/roles")]
    public async Task<IActionResult> UpdateUserRoles(string id, [FromBody] List<string> roles)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound($"User with ID {id} not found");

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return BadRequest(removeResult.Errors);

            if (roles.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, roles);
                if (!addResult.Succeeded)
                    return BadRequest(addResult.Errors);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating roles for user {UserId}", id);
            return StatusCode(500, "Internal server error while updating user roles");
        }
    }

    // PUT: api/users/{id}/company
    [HttpPut("{id}/company")]
    public async Task<IActionResult> UpdateUserCompany(string id, [FromBody] string companyId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound($"User with ID {id} not found");

            if (!Guid.TryParse(companyId, out Guid companyGuid))
                return BadRequest("Invalid company ID format");

            var company = await _context.Companies.FindAsync(companyGuid);
            if (company == null) return NotFound($"Company with ID {companyId} not found");

            user.CompanyGuid = companyGuid;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating company for user {UserId}", id);
            return StatusCode(500, "Internal server error while updating user company");
        }
    }

    // POST: api/users/{id}/change-password
    [HttpPost("{id}/change-password")]
    public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePasswordDto dto)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound($"User with ID {id} not found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", id);
            return StatusCode(500, "Internal server error while changing password");
        }
    }
}