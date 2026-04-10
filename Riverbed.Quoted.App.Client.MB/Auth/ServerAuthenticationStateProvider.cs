using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Riverbed.Quoted.App.MB.Auth;

public sealed class ServerAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private readonly ServerAuthenticationClient _authenticationClient;
    private CurrentUserResponse? _currentUser;

    public ServerAuthenticationStateProvider(ServerAuthenticationClient authenticationClient)
    {
        _authenticationClient = authenticationClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            _currentUser ??= await _authenticationClient.GetCurrentUserAsync();
        }
        catch
        {
            _currentUser = null;
        }

        return CreateAuthenticationState(_currentUser);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        LoginResponse loginResponse;
        try
        {
            loginResponse = await _authenticationClient.LoginAsync(request);
        }
        catch
        {
            return new LoginResponse(false, false, false);
        }

        if (!loginResponse.Succeeded)
        {
            return loginResponse;
        }

        try
        {
            _currentUser = await _authenticationClient.GetCurrentUserAsync();
        }
        catch
        {
            _currentUser = null;
        }

        NotifyAuthenticationStateChanged(Task.FromResult(CreateAuthenticationState(_currentUser)));
        return loginResponse;
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _authenticationClient.LogoutAsync();
        }
        catch
        {
        }

        _currentUser = null;
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    private static AuthenticationState CreateAuthenticationState(CurrentUserResponse? currentUser)
    {
        if (currentUser is null)
        {
            return Anonymous;
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, currentUser.UserId),
            new(ClaimTypes.Name, currentUser.Email),
            new(ClaimTypes.Email, currentUser.Email)
        };

        claims.AddRange(currentUser.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var identity = new ClaimsIdentity(claims, authenticationType: "ServerCookie");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
}
