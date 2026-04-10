namespace Riverbed.Quoted.App.MB.Auth;

public sealed class CurrentUserResponse
{
    public required string UserId { get; set; }
    public required string Email { get; set; }
    public List<string> Roles { get; set; } = [];
    public bool IsDarkMode { get; set; }
}
