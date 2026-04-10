using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net;
using System.Net.Http.Json;

namespace Riverbed.Quoted.App.MB.Auth;

public sealed class ServerAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState Unauthenticated =
        new(new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity()));

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ServerAuthenticationStateProvider> _logger;

    public ServerAuthenticationStateProvider(IHttpClientFactory httpClientFactory, ILogger<ServerAuthenticationStateProvider> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(logger);
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var client = _httpClientFactory.CreateClient("ServerAuth");

        using var request = new HttpRequestMessage(HttpMethod.Get, "api/UserInfo/Current");
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        try
        {
            using var response = await client.SendAsync(request);
            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                return Unauthenticated;
            }

            response.EnsureSuccessStatusCode();
            var currentUser = await response.Content.ReadFromJsonAsync<CurrentUserResponse>();
            if (currentUser is null)
            {
                return Unauthenticated;
            }

            var principal = new UserInfo
            {
                UserId = currentUser.UserId,
                Email = currentUser.Email,
                Roles = currentUser.Roles,
                IsDarkMode = currentUser.IsDarkMode
            }.ToClaimsPrincipal();

            return new AuthenticationState(principal);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Unable to retrieve the current user from the server authentication endpoint.");
            return Unauthenticated;
        }
        catch (NotSupportedException ex)
        {
            _logger.LogWarning(ex, "Unable to parse server authentication payload.");
            return Unauthenticated;
        }
        catch (System.Text.Json.JsonException ex)
        {
            _logger.LogWarning(ex, "Invalid JSON returned by server authentication endpoint.");
            return Unauthenticated;
        }
    }

    /// <summary>
    /// Refreshes the authentication state from the server and notifies subscribers.
    /// </summary>
    public Task RefreshAuthenticationStateAsync()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return Task.CompletedTask;
    }
}
