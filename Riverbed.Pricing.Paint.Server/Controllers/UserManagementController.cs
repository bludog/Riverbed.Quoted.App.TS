[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class UserManagementController : ControllerBase
{
    private readonly IPricingService _pricingService;

    public UserManagementController(IPricingService pricingService)
    {
        _pricingService = pricingService;
    }

    [HttpGet("roles")]
    public async Task<ActionResult<List<string>>> GetAvailableRoles()
    {
        return await _pricingService.GetAvailableRolesAsync();
    }

    [HttpGet("companies")]
    public async Task<ActionResult<List<CompanyDto>>> GetCompanies()
    {
        return await _pricingService.GetCompaniesAsync();
    }

    [HttpPut("user")]
    public async Task<IActionResult> UpdateUser(UserDto user)
    {
        var result = await _pricingService.UpdateUserAsync(user);
        return result ? Ok() : BadRequest();
    }

    [HttpPut("user/roles")]
    public async Task<IActionResult> UpdateUserRoles(UserRoleUpdateDto update)
    {
        var result = await _pricingService.UpdateUserRolesAsync(update.UserId, update.Roles);
        return result ? Ok() : BadRequest();
    }

    [HttpPut("user/company")]
    public async Task<IActionResult> UpdateUserCompany(UserCompanyUpdateDto update)
    {
        var result = await _pricingService.UpdateUserCompanyAsync(update.UserId, update.CompanyId);
        return result ? Ok() : BadRequest();
    }
}