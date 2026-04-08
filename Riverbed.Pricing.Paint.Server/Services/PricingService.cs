public class PricingService : IPricingService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public PricingService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<List<string>> GetAvailableRolesAsync()
    {
        return await _roleManager.Roles
            .Select(r => r.Name)
            .Where(n => n != null)
            .ToListAsync();
    }

    public async Task<List<CompanyDto>> GetCompaniesAsync()
    {
        return await _context.Companies
            .Select(c => new CompanyDto
            {
                Id = c.Id.ToString(),
                Name = c.Name
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateUserAsync(UserDto userDto)
    {
        var user = await _userManager.FindByIdAsync(userDto.Id);
        if (user == null) return false;

        user.UserName = userDto.UserName;
        user.Email = userDto.Email;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UpdateUserRolesAsync(string userId, List<string> roles)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        var result = await _userManager.AddToRolesAsync(user, roles);
        
        return result.Succeeded;
    }

    public async Task<bool> UpdateUserCompanyAsync(string userId, string companyId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.CompanyId = Guid.Parse(companyId);
        var result = await _userManager.UpdateAsync(user);
        
        return result.Succeeded;
    }
}