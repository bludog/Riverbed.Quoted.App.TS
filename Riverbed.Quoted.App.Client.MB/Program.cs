using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Riverbed.Quoted.App.MB.Auth;
using Riverbed.Quoted.App.MB;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var localAuthSection = builder.Configuration.GetSection("Local");
var apiBaseAddress = localAuthSection["ApiBaseUrl"];

if (string.IsNullOrWhiteSpace(apiBaseAddress))
{
    throw new InvalidOperationException("Web API base URL is not configured. Set Local:ApiBaseUrl in wwwroot/appsettings.json.");
}

if (!Uri.TryCreate(apiBaseAddress, UriKind.Absolute, out var apiBaseUri))
{
    throw new InvalidOperationException("Web API base URL is invalid. Set Local:ApiBaseUrl to an absolute URL.");
}

builder.Services.AddMudServices();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<ServerCookieCredentialsHandler>();
builder.Services.AddScoped<ServerAuthenticationClient>();
builder.Services.AddScoped<ServerAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<ServerAuthenticationStateProvider>());

builder.Services.AddHttpClient("ServerAPI", client =>
    {
        client.BaseAddress = apiBaseUri;
    })
    .AddHttpMessageHandler<ServerCookieCredentialsHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));

await builder.Build().RunAsync();
