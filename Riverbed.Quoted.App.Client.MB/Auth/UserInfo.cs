using System.Security.Claims;

namespace Riverbed.Quoted.App.MB.Auth;

public sealed class UserInfo
{
    public required string UserId { get; set; }
    public required string Email { get; set; }
    public List<string> Roles { get; set; } = [];
    public bool IsDarkMode { get; set; }

    public ClaimsPrincipal ToClaimsPrincipal()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, UserId),
            new(ClaimTypes.Name, Email),
            new(ClaimTypes.Email, Email),
            new("IsDarkMode", IsDarkMode.ToString().ToLowerInvariant())
        };

        claims.AddRange(Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: "ServerCookie"));
    }
}
