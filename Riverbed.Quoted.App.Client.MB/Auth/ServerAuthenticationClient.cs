using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net;
using System.Net.Http.Json;

namespace Riverbed.Quoted.App.MB.Auth;

public sealed class ServerAuthenticationClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ServerAuthenticationClient(IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Sends login credentials to the server and receives an authentication result.
    /// </summary>
    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var client = _httpClientFactory.CreateClient("ServerAuth");

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/authentication/login")
        {
            Content = JsonContent.Create(request)
        };

        httpRequest.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        using var response = await client.SendAsync(httpRequest, cancellationToken);

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.BadRequest)
        {
            return await ReadLoginResponseAsync(response, cancellationToken) ?? new LoginResponse(false, false, false);
        }

        response.EnsureSuccessStatusCode();

        return await ReadLoginResponseAsync(response, cancellationToken)
            ?? throw new InvalidOperationException("Server authentication endpoint returned an empty payload.");
    }

    /// <summary>
    /// Signs out the current user through the server authentication endpoint.
    /// </summary>
    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("ServerAuth");

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/authentication/logout");
        httpRequest.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        using var response = await client.SendAsync(httpRequest, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return;
        }

        response.EnsureSuccessStatusCode();
    }

    private static async Task<LoginResponse?> ReadLoginResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        return await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);
    }
}
