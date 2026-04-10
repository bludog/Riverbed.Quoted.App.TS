namespace Riverbed.Quoted.App.MB.Auth;

public sealed record LoginRequest(
    string Email,
    string Password,
    bool RememberMe);
