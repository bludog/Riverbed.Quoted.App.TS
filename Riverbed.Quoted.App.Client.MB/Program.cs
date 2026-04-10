using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Blazorise.LoadingIndicator;
using Blazorise.RichTextEdit;
using DevExpress.Blazor;
using MudBlazor.Services;
using Radzen;
using Riverbed.Pricing.Paint.Client;
using Riverbed.Pricing.Paint.Client.Pages.Components.EmailEditor;
using Riverbed.Pricing.Paint.Shared.Services;
using Riverbed.Quoted.App.MB.Auth;

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
builder.Services.AddDevExpressBlazor();
builder.Services.AddBlazorise(options =>
{
    options.Immediate = true;
})
    .AddLoadingIndicator()
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons()
    .AddBlazoriseRichTextEdit();

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddScoped<Riverbed.Pricing.Paint.Shared.Services.ThemeService>();
builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddTransient<IEmailEditorTemplateManger, EmailEditorTemplateManger>();

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
