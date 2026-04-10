namespace Riverbed.Quoted.App.MB.Auth;

public sealed record LoginResponse(
    bool Succeeded,
    bool RequiresTwoFactor,
    bool IsLockedOut);
