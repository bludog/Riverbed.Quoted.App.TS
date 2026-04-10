using System.Net.Http.Json;

namespace Riverbed.Quoted.App.MB.Auth;

public sealed class ServerAuthenticationClient
{
    private readonly HttpClient _httpClient;

    public ServerAuthenticationClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ServerAPI");
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("authentication/login", request, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken))
                   ?? new LoginResponse(false, false, false);
        }

        return (await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken))
               ?? new LoginResponse(false, false, false);
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        await _httpClient.PostAsync("authentication/logout", content: null, cancellationToken);
    }

    public async Task<CurrentUserResponse?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("UserInfo/Current", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<CurrentUserResponse>(cancellationToken);
    }
}
