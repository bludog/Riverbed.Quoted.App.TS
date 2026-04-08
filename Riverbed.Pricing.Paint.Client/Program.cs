using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Blazorise.LoadingIndicator;
using Blazorise.RichTextEdit;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using Radzen;
using Riverbed.Pricing.Paint.Client;
using Riverbed.Pricing.Paint.Reports.Utils;
using Riverbed.Pricing.Paint.Shared.Services;
using System.Net.Http.Headers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
builder.Services.AddScoped<FileSaveHelper>();
builder.Services.AddSingleton<IPricingService, PricingService>();
builder.Services.AddDevExpressBlazor();
builder.Services.AddApiAuthorization();

// Radzen services required by various components (Dialog, Notification, Tooltip, ContextMenu)
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

builder.Services.AddSingleton<Riverbed.Pricing.Paint.Shared.Services.ThemeService>();

builder.Services.AddBlazorise(options =>
{
    options.Immediate = true;
})   
    .AddLoadingIndicator()
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons()
    .AddBlazoriseRichTextEdit();

builder.Services.AddMudServices();

builder.Services.AddScoped(sp =>
{
    string baseAddress = builder.HostEnvironment.BaseAddress;

    var httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
    httpClient.DefaultRequestHeaders.Accept.Clear();
    httpClient.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
    httpClient.DefaultRequestHeaders.Add("User-Agent", "Bludog Software");

    return httpClient;
});

await builder.Build().RunAsync();
