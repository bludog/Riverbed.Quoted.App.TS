using Microsoft.AspNetCore.Mvc;
using DevExpress.XtraRichEdit;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Riverbed.Pricing.Paint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportDownloadController : ControllerBase
    {
        [HttpGet("GenerateProjectPdf/{projectGuid}")]
        public async Task<IActionResult> GeneratePdfFromWebPage(string projectGuid)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(projectGuid))
                {
                    return BadRequest("Project GUID cannot be null or empty.");
                }

                // Get the base URL from the request context
                var request = HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";
                string url = $"{baseUrl}/projectquotereport/{projectGuid}";

                // Fetch HTML content from the URL
                string htmlContent;
                using (var httpClient = new HttpClient())
                {
                    htmlContent = await httpClient.GetStringAsync(url);
                }

                // Embed CSS and JavaScript into the HTML
                string embeddedHtml = await EmbedResourcesIntoHtml(htmlContent, baseUrl);

                // Convert HTML to PDF using DevExpress RichEditDocumentServer
                using (var richEditDocumentServer = new RichEditDocumentServer())
                {
                    richEditDocumentServer.HtmlText = embeddedHtml;

                    // Create a MemoryStream to hold the PDF content
                    using (var memoryStream = new MemoryStream())
                    {
                        richEditDocumentServer.ExportToPdf(memoryStream);

                        // Convert the MemoryStream to a byte array
                        byte[] pdfBytes = memoryStream.ToArray();

                        // Return the PDF as a downloadable file
                        return File(pdfBytes, "application/pdf", $"ProjectQuote_{projectGuid}.pdf");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating PDF: {ex.Message}");
            }
        }

        private async Task<string> EmbedResourcesIntoHtml(string htmlContent, string baseUrl)
        {
            baseUrl = baseUrl.TrimEnd('/') + "/"; // Ensure base URL ends with a slash
            // List of CSS files to fetch
            var cssFiles = new List<string>
            {
                $"{baseUrl}bootstrap/bootstrap.min.css",
                $"{baseUrl}app.css",
                $"{baseUrl}Riverbed.Pricing.Paint.styles.css",
                $"{baseUrl}_content/Microsoft.Fast.Components.FluentUI/css/reboot.css",
                $"{baseUrl}_content/Microsoft.Fast.Components.FluentUI/css/variables.css",
                $"{baseUrl}_content/BlazorWheelPicker/BlazorWheelPicker.css",
                $"{baseUrl}_content/DevExpress.Blazor.Themes/blazing-berry.bs5.min.css",
                $"{baseUrl}_content/DevExpress.Blazor.Reporting.Viewer/css/dx-blazor-reporting-components.bs5.css",
                $"{baseUrl}_content/DevExpress.Blazor.Themes/bootstrap-external.bs5.min.css",
                $"{baseUrl}_content/Blazorise/blazorise.css?v=1.7.3.0",
                $"{baseUrl}_content/Blazorise.Bootstrap5/blazorise.bootstrap5.css?v=1.7.3.0",
                $"{baseUrl}_content/Blazorise.LoadingIndicator/blazorise.loadingindicator.css",
                $"{baseUrl}_content/Blazorise.Snackbar/blazorise.snackbar.css",
                $"{baseUrl}_content/Blazorise.Icons.Material/blazorise.icons.material.css"
            };

            // Fetch and embed CSS files
            var sb = new StringBuilder();
            sb.Append("<!DOCTYPE html>");
            sb.Append("<html>");
            sb.Append("<head>");
            sb.Append("<meta charset=\"utf-8\">");
            sb.Append("<title>Project Quote</title>");
            sb.Append("<style>");

            foreach (var cssFile in cssFiles)
            {
                string cssContent = await FetchResource(cssFile);
                sb.Append(cssContent); // Embed CSS content
            }

            sb.Append("</style>");
            sb.Append("</head>");
            sb.Append("<body>");
            sb.Append(htmlContent); // Embed the original HTML content
            sb.Append("</body>");
            sb.Append("</html>");

            return sb.ToString();
        }

        private async Task<string> FetchResource(string url)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    return await httpClient.GetStringAsync(url);
                }
                catch
                {
                    return string.Empty; // Return empty string if the resource cannot be fetched
                }
            }
        }
    }
}