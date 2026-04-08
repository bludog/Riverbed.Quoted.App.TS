using DevExpress.XtraRichEdit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuppeteerSharp;
using Riverbed.Pricing.Paint.Reports;
using Riverbed.Pricing.Paint.Shared.Data;
using System;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp.Media;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System.Collections.Generic;
using System.Linq;

namespace Riverbed.Pricing.Paint.Controllers.Reporting
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : Controller
    {
        private readonly PricingDbContext _context;
        private readonly ILogger<ReportController> _logger;

        public ReportController(PricingDbContext context, ILogger<ReportController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("Interior/{projectGuidId}")]
        public async Task<IActionResult> GetInteriorQuoteAsPdf(string projectGuidId)
        {
            try
            {
                if (!Guid.TryParse(projectGuidId, out Guid projectGuid))
                {
                    return BadRequest("Invalid project Guid ID format.");
                }

                var projectData = _context.Projects
                    .FirstOrDefault(p => p.ProjectGuid == projectGuid);

                if (projectData == null)
                {
                    return NotFound("Project not found.");
                }

                string baseUrl = GetBaseUrl();
                string quoteViewerUrl = $"{baseUrl}projectquotereport/{projectGuid}";
                _logger.LogInformation("Generating PDF for project {ProjectId} at URL: {QuoteViewerUrl}", projectGuid, quoteViewerUrl);

                string fileName = $"Quote_{projectData.ProjectName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf";

                var pdfBytes = await GeneratePdfAsync(quoteViewerUrl);
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF for project {ProjectId}", projectGuidId);
                return StatusCode(500, $"Error generating PDF: {ex.Message}");
            }
        }

        private async Task<byte[]> GeneratePdfAsync(string url)
        {
            try
            {
                _logger.LogInformation("Initializing PDF generation for URL: {Url}", url);

                var chromiumPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "chromium", "chrome-headless-shell.exe");
                await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    ExecutablePath = chromiumPath,
                    Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
                });

                await using var page = await browser.NewPageAsync();

                // Set viewport for consistent rendering
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 1240,  // A4 width at 96 DPI
                    Height = 1754  // A4 height at 96 DPI
                });

                // Navigate to page with proper wait conditions
                await page.GoToAsync(url, new NavigationOptions
                {
                    WaitUntil = new[] 
                    { 
                        WaitUntilNavigation.DOMContentLoaded,
                        WaitUntilNavigation.Load,
                        WaitUntilNavigation.Networkidle0 
                    },
                    Timeout = 30000
                });

                // Small delay for Blazor rendering
                await Task.Delay(1000);

                _logger.LogInformation("Generating PDF...");

                // Generate PDF with optimal settings
                var pdfData = await page.PdfDataAsync(new PdfOptions
                {
                    Format = PaperFormat.A4,
                    PrintBackground = true,
                    PreferCSSPageSize = true,
                    MarginOptions = new MarginOptions
                    {
                        Top = "0mm",
                        Bottom = "0mm",
                        Left = "0mm",
                        Right = "0mm"
                    }
                });

                _logger.LogInformation("PDF generated successfully, size: {Size} bytes", pdfData.Length);
                return pdfData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF from URL: {Url}", url);
                throw;
            }
        }

        // Get Quote as PDF from HTML report
        [HttpGet("typeId/{reportTypeId:int}/companyGuid/{companyGuid:guid}")]
        public async Task<IActionResult> CreateReportPDF(int reportTypeId, string companyGuid)
        {
            try
            {
                var report = _context.CompanyHTMLReports
                    .Where(r => r.CompanyGuid == Guid.Parse(companyGuid) && r.ReportTypeId == reportTypeId)
                    .FirstOrDefault();
                var reportName = _context.CompanyReportTypes
                    .Where(rt => rt.Id == report.Id)
                    .Select(rt => rt.ReportTypeName)
                    .FirstOrDefault();

                if (report == null)
                {
                    _logger.LogWarning($"HTML report with ID: {reportName} not found");
                    return NotFound();
                }

                _logger.LogInformation($"HTML report with ID: {reportName} retrieved successfully");
                using (var wordProcessor = new RichEditDocumentServer())
                {
                    wordProcessor.HtmlText = report.ReportHTMLText;

                    using (var pdfStream = new MemoryStream())
                    {
                        wordProcessor.ExportToPdf(pdfStream);
                        pdfStream.Position = 0; // Reset stream position
                        var pdfBytes = pdfStream.ToArray();
                        // Return the PDF file as a downloadable file
                        return File(pdfBytes, "application/pdf", reportName);
                    }
                }
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, $"Invalid GUID format: {companyGuid}");
                return BadRequest("Invalid GUID format");
            }
        }

        [HttpGet("Project/QuotePage/{projectGuid}")]
        public async Task<IActionResult> GetCustomerProjectQuoteVIPdf(string projectGuid)
        {
            if (!Guid.TryParse(projectGuid, out Guid guid))
                return BadRequest("Invalid projectGuid format.");

            // Instantiate the report
            var report = new CustomerProjectQuoteVI();

            // Set the parameter (ensure the parameter name matches the report)
            report.Parameters["CustomerProjectGuid"].Value = guid;
            report.Parameters["CustomerProjectGuid"].Visible = false;

            // Generate the document
            report.CreateDocument();

            // Export to PDF in memory
            using var ms = new MemoryStream();
            report.ExportToPdf(ms);
            ms.Position = 0;

            // Return as a file
            return File(ms.ToArray(), "application/pdf", $"CustomerProjectQuote_{guid}.pdf");
        }

        [HttpGet("combinedpdf/{projectGuid}")]
        public async Task<IActionResult> GetCombinedPdf(string projectGuid)
        {
            if (!Guid.TryParse(projectGuid, out Guid guid))
                return BadRequest("Invalid projectGuid format.");

            // Define the report sections and their URLs
            var baseUrl = GetBaseUrl();
            var sections = new[]
            {
                // Call the following for each Page company-html-report-display/{ProjectGuid:guid}/{ReportTypeId:int}
                new { Title = "Welcome", Url = $"{baseUrl}reports/company-html-report-page/{projectGuid}/4" },
                new { Title = "Insurance", Url = $"{baseUrl}reports/company-html-report-page/{projectGuid}/3" },
                new { Title = "Quote", Url = $"{baseUrl}reports/projectquotereport/{projectGuid}/" },
                new { Title = "Agreement", Url = $"{baseUrl}reports/company-html-report-page/{projectGuid}/1" },
                new { Title = "Warranty", Url = $"{baseUrl}reports/company-html-report-page/{projectGuid}/6" },
                new { Title = "Signatures", Url = $"{baseUrl}reports/company-html-report-page/{projectGuid}/5" }
            };

            var pdfStreams = new List<byte[]>();
            foreach (var section in sections)
            {
                var pdfBytes = await GeneratePdfAsync(section.Url);
                pdfStreams.Add(pdfBytes);
            }

            // Merge PDFs using PdfSharpCore
            var outputPdf = new PdfSharpCore.Pdf.PdfDocument();
            foreach (var pdfBytes in pdfStreams)
            {
                using var ms = new MemoryStream(pdfBytes);
                var inputPdf = PdfSharpCore.Pdf.IO.PdfReader.Open(ms, PdfSharpCore.Pdf.IO.PdfDocumentOpenMode.Import);
                for (int i = 0; i < inputPdf.PageCount; i++)
                {
                    outputPdf.AddPage(inputPdf.Pages[i]);
                }
            }
            using var outStream = new MemoryStream();
            outputPdf.Save(outStream, false);
            return File(outStream.ToArray(), "application/pdf", "CombinedReport.pdf");
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
    }
}
