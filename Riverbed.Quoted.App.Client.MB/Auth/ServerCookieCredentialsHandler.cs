using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Riverbed.Quoted.App.MB.Auth;

public sealed class ServerCookieCredentialsHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return base.SendAsync(request, cancellationToken);
    }
}
