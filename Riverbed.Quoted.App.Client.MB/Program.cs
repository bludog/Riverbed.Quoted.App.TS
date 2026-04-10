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

var serverBaseAddress = builder.HostEnvironment.BaseAddress;
if (!string.IsNullOrWhiteSpace(apiBaseAddress) && Uri.TryCreate(apiBaseAddress, UriKind.Absolute, out var configuredApiBaseUri))
{
    serverBaseAddress = configuredApiBaseUri.AbsoluteUri;
}

builder.Services.AddMudServices();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddScoped<ServerAuthenticationClient>();
builder.Services.AddTransient<ServerCookieCredentialsHandler>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddHttpClient("ServerAuth", client =>
    {
        client.BaseAddress = new Uri(serverBaseAddress);
    })
    .AddHttpMessageHandler<ServerCookieCredentialsHandler>();

builder.Services.AddHttpClient("AuthorizedAPI", client =>
    {
        client.BaseAddress = new Uri(serverBaseAddress);
    })
    .AddHttpMessageHandler<ServerCookieCredentialsHandler>();

await builder.Build().RunAsync();
