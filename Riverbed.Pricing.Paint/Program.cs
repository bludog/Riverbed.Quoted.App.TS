using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Blazorise.Icons.Material;
using Blazorise.LoadingIndicator;
using Blazorise.RichTextEdit;
using DevExpress.AspNetCore.Reporting;
using DevExpress.Blazor;
using DevExpress.Blazor.Reporting;
using DevExpress.XtraReports.Web.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MudBlazor.Services;
using Radzen; // Added for Radzen services
using Riverbed.Pricing.Paint.Client.Pages;
using Riverbed.Pricing.Paint.Client.Pages.Components.EmailEditor;
using Riverbed.Pricing.Paint.Components;
using Riverbed.Pricing.Paint.Components.Account;
using Riverbed.Pricing.Paint.Components.Pages;
using Riverbed.Pricing.Paint.Controllers;
using Riverbed.Pricing.Paint.Controllers.Paint;
using Riverbed.Pricing.Paint.Logging;
using Riverbed.Pricing.Paint.Reports.Utils;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Services;
using Riverbed.PricingEngines;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using System.Reflection;
using System.Security.Claims;
using Tewr.Blazor.FileReader;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();

        // Add Serilog
        builder.Host.UseSerilog((context, services, configuration) =>
     configuration
         .ReadFrom.Configuration(context.Configuration)
         .ReadFrom.Services(services)
         .Enrich.FromLogContext()
         .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
         .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Fatal) // ultra-suppress EF SQL
         .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Infrastructure", LogEventLevel.Warning));
        

        builder.Services.AddDevExpressBlazor();
        builder.Services.AddDevExpressBlazorReporting();
        builder.Services.AddDevExpressServerSideBlazorReportViewer();

        builder.WebHost.UseWebRoot("wwwroot");
        builder.WebHost.UseStaticWebAssets();

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
        builder.Services.AddScoped<FileSaveHelper>();
        builder.Services.AddScoped<PaintPricingEngine>();
        builder.Services.AddScoped<PaintPricingEngineController>();
        builder.Services.AddScoped<CompanyProvisioningController>();
        // Replace transient registration with scoped factory that forwards auth headers to HttpClient and validates roles client-side
        builder.Services.AddScoped<IPricingService, PricingService>();
        builder.Services.AddScoped<Riverbed.Pricing.Paint.Shared.EmailService.IEmailService, Riverbed.Pricing.Paint.Shared.EmailService.EmailService>();
        builder.Services.AddTransient<IEmailEditorTemplateManger, EmailEditorTemplateManger>();

        // Radzen component services for dialogs, notifications, tooltips, context menus
        builder.Services.AddScoped<DialogService>();
        builder.Services.AddScoped<NotificationService>();
        builder.Services.AddScoped<TooltipService>();
        builder.Services.AddScoped<ContextMenuService>();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents(); 

        // Radzen services required by various components (Dialog, Notification, Tooltip, ContextMenu)
        builder.Services.AddScoped<DialogService>();
        builder.Services.AddScoped<NotificationService>();
        builder.Services.AddScoped<TooltipService>();
        builder.Services.AddScoped<ContextMenuService>();

        //// Register ServiceTitanClient with HttpClient
        //builder.Services.AddHttpClient<ServiceTitanClient>(client =>
        //    {
        //    client.BaseAddress = new Uri(builder.Configuration["ServiceTitanService:Url"] ?? "https://localhost:7001");
        //});

        builder.Services.AddScoped<Riverbed.Pricing.Paint.Shared.Services.ThemeService>();

        builder.Services.AddBlazorise(options =>
        {
            options.Immediate = true;
        })
            .AddBootstrap5Providers()
            .AddLoadingIndicator()
            .AddMaterialIcons()
            .AddFontAwesomeIcons(); // Removed .AddToast()
        builder.Services.AddBlazoriseRichTextEdit(options =>
        {
        });

        builder.Services.AddDevExpressBlazor(configure => configure.BootstrapVersion = BootstrapVersion.v5);

        builder.Services.AddHttpClient();
        builder.Services.AddAuthorization();
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Configure password settings
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            // Configure lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        //builder.Services.AddAuthentication(options =>
        //    {
        //        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        //        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        //    })
        //    .AddIdentityCookies();

        // Register the HttpClient and FileReader services
        builder.Services.AddFileReaderService(options => { });

        // Register HttpClient for API calls
        builder.Services.AddScoped(sp =>
        {
            string baseAddress = builder.Environment.IsDevelopment()
                ? "https://localhost:7027/"
                : "https://bludog-software.com/";

            var httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Bludog Software");

            return httpClient;
        });

        // Add HttpContextAccessor service
        builder.Services.AddHttpContextAccessor();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDbContext<PricingDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();


        builder.Services.AddRazorPages();   // needed if you ever use the built-in /Identity UI


        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        builder.Services.AddControllers(); // Add support for controllers
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", policy =>
            {
                policy.WithOrigins("https://localhost", "https://bludog-software.com")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        // Register the Swagger generator, defining one or more Swagger documents
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Riverbed Pricing API", Version = "v1" });

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        // Configure DevExpress reporting services
        builder.Services.ConfigureReportingServices(configurator =>
        {
            configurator.ConfigureWebDocumentViewer(viewerConfigurator =>
            {
                viewerConfigurator.UseCachedReportSourceBuilder();
            });
        });

        builder.Services.AddMudServices();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // Serilog request logging + enrich with UserName and RequestPath (for the request log entry)
        //app.UseSerilogRequestLogging(options =>
        //{
        //    options.EnrichDiagnosticContext = (ctx, http) =>
        //    {
        //        ctx.Set("RequestPath", http.Request?.Path.Value);

        //        var userName =
        //            (http.User?.Identity?.IsAuthenticated == true
        //                ? (http.User.Identity?.Name
        //                   ?? http.User.FindFirst(ClaimTypes.Email)?.Value
        //                   ?? http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
        //                : "anonymous") ?? "anonymous";

        //        ctx.Set("UserName", userName);
        //    };
        //});

        // Push properties into LogContext for the duration of each request
        app.Use(async (http, next) =>
        {
            var userName =
                (http.User?.Identity?.IsAuthenticated == true
                    ? (http.User.Identity?.Name
                       ?? http.User.FindFirst(ClaimTypes.Email)?.Value
                       ?? http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                    : "anonymous") ?? "anonymous";

            var requestPath = http.Request?.Path.Value ?? string.Empty;

            using (LogContext.PushProperty("UserName", userName))
            using (LogContext.PushProperty("RequestPath", requestPath))
            {
                await next();
            }
        });

        // Replace invalid X-Frame-Options header usage with a proper CSP frame-ancestors directive
        app.Use(async (context, next) =>
        {
            // Remove any X-Frame-Options header to avoid invalid/legacy directives
            context.Response.Headers.Remove("X-Frame-Options");
            // Allow this app's pages to be framed only by these ancestors
            context.Response.Headers["Content-Security-Policy"] =
                "frame-ancestors 'self' https://www.builderhomeupgrades.com https://localhost https://bludog-software.com";
            await next.Invoke();
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Riverbed Pricing API V1"));
        }


        app.UseDevExpressBlazorReporting();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseCors("AllowSpecificOrigins");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();

        app.UseDevExpressBlazorReporting();

        app.MapControllers();
        app.MapRazorComponents<App>()
            // .net9.0 fix to remove compression and app will work
            .AddInteractiveServerRenderMode(o => o.DisableWebSocketCompression = true)
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Riverbed.Pricing.Paint.Client.App).Assembly); ;

        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();

        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Console.WriteLine("UNHANDLED: " + e.ExceptionObject);
        };
        app.Logger.LogInformation("App started in {Env}", app.Environment.EnvironmentName);


        app.Run();
    }
}