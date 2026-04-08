using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Extensions;
using PuppeteerSharp;
using System;
using PuppeteerSharp.Media;
using System.Text.Json;

namespace Riverbed.Pricing.Paint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotePdfController : ControllerBase
    {
        private readonly ILogger<QuotePdfController> _logger;
        private readonly PricingDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public QuotePdfController(ILogger<QuotePdfController> logger, PricingDbContext dbContext, IConfiguration configuration)
        {
            _logger = logger;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        private string GetBaseUrl()
        {
            // Generate the base URL using the current request
            var request = HttpContext.Request;
            
            // Build the base URL including the scheme and host
            var baseUrl = $"{request.Scheme}://{request.Host}";
            
            // Add the PathBase which handles applications in subfolders/virtual directories
            if (!string.IsNullOrEmpty(request.PathBase))
            {
                baseUrl += request.PathBase;
            }

            // Ensure the URL ends with a trailing slash
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }
            
            _logger.LogInformation("Generated base URL: {BaseUrl}", baseUrl);
            return baseUrl;
        }

        [HttpGet("Interior/{projectId}")]
        public async Task<IActionResult> GetInteriorQuoteAsPdf(string projectId)
        {
            try
            {
                // Validate projectId is a valid GUID
                if (!Guid.TryParse(projectId, out Guid projectGuid))
                {
                    return BadRequest("Invalid project ID format.");
                }

                // Get the project data to use for the filename
                var projectData = await _dbContext.Projects
                    .FirstOrDefaultAsync(p => p.ProjectGuid == projectGuid);

                if (projectData == null)
                {
                    return NotFound("Project not found.");
                }

                // Get the rendered HTML from the page URL
                string baseUrl = GetBaseUrl();
                string quoteViewerUrl = $"{baseUrl}projectquotereport/{projectId}";
                _logger.LogInformation("Generating PDF for project {ProjectId} at URL: {QuoteViewerUrl}", projectId, quoteViewerUrl);

                // Generate the PDF using PuppeteerSharp
                byte[] pdfData = await GeneratePdfFromUrl(quoteViewerUrl);
                
                // Generate filename based on project name and current date
                string fileName = $"Quote_{projectData.ProjectName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf";
                
                // Return PDF as downloadable file
                return File(pdfData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF for project {ProjectId}", projectId);
                return StatusCode(500, $"Error generating PDF: {ex.Message}");
            }
        }

        private async Task<byte[]> GeneratePdfFromUrl(string url)
        {
            // Specify a custom path for Chromium download
            var browserFetcherOptions = new BrowserFetcherOptions
            {
                Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Chrome") // Ensure this directory exists
            };
            var browserFetcher = new BrowserFetcher(browserFetcherOptions);

            // Specify a known Chromium revision (e.g., "1022525") as a string
            const string chromiumRevision = "1022525";

            // Download Chromium for the specified revision
            await browserFetcher.DownloadAsync(chromiumRevision);

            // Add additional query parameter to signal this is for PDF generation
            if (!url.Contains("?"))
                url += "?printMode=true";
            else
                url += "&printMode=true";

            // Launch browser
            var launchOptions = new LaunchOptions
            {
                Headless = true,
                ExecutablePath = browserFetcher.GetExecutablePath(chromiumRevision), // Use the downloaded Chromium
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
            };

            using var browser = await Puppeteer.LaunchAsync(launchOptions);
            using var page = await browser.NewPageAsync();

            // Set viewport size to match A4 dimensions (roughly 8.27 × 11.69 inches)
            await page.SetViewportAsync(new ViewPortOptions
            {
                Width = 1240,
                Height = 1754
            });

            // Navigate to the page with a better waiting strategy
            await page.GoToAsync(url, new NavigationOptions
            {
                WaitUntil = new[] { WaitUntilNavigation.Networkidle0, WaitUntilNavigation.Load, WaitUntilNavigation.DOMContentLoaded },
                Timeout = 60000 // 60 second timeout
            });

            // Wait for a reasonable time for any remaining JavaScript to complete
            await Task.Delay(5000);

            _logger.LogInformation("Page loaded, preparing to generate PDF");

            // Generate PDF with minimal margins to maximize content area
            var pdfData = await page.PdfDataAsync(new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                PreferCSSPageSize = true,
                MarginOptions = new MarginOptions
                {
                    Top = "0mm",     // Removed margins completely
                    Bottom = "0mm",  // Removed margins completely
                    Left = "0mm",    // Removed margins completely
                    Right = "0mm"    // Removed margins completely
                }
            });

            _logger.LogInformation($"PDF generated successfully, size: {pdfData.Length} bytes");
            return pdfData;
        }
        
        // Define a class to properly map the JSON result
        private class PageDimensions
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }
    }
}