using DevExpress.XtraRichEdit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Riverbed.Pricing.Paint.Controllers.Utils;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;
using Riverbed.Pricing.Paint.Shared.Entities.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Controllers.Reporting
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyHTMLReportsController : ControllerBase
    {
        private readonly PricingDbContext _context;
        private readonly ILogger<CompanyHTMLReportsController> _logger;

        public CompanyHTMLReportsController(PricingDbContext context, ILogger<CompanyHTMLReportsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // New: Merge arbitrary HTML with tokens for a given company and project
        // POST: api/CompanyHTMLReports/merge-html
        [HttpPost("merge-html")]
        public async Task<ActionResult<string>> MergeHtml([FromBody] MergeHtmlRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Html))
                return BadRequest("Html body is required.");

            if (!Guid.TryParse(request.CompanyGuid, out var companyGuid))
                return BadRequest("Invalid companyGuid.");

            if (!Guid.TryParse(request.ProjectGuid, out var projectGuid))
                return BadRequest("Invalid projectGuid.");

            try
            {
                var mergeUtil = new TemplateMergeUtility(_context, _logger);
                var merged = await mergeUtil.MergeTemplateHtmlAsync(request.Html, companyGuid, projectGuid);
                return Ok(merged);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error merging HTML for company {CompanyGuid} and project {ProjectGuid}", request.CompanyGuid, request.ProjectGuid);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while merging the HTML template.");
            }
        }

        public class MergeHtmlRequest
        {
            public string CompanyGuid { get; set; } = string.Empty;
            public string ProjectGuid { get; set; } = string.Empty;
            public string Html { get; set; } = string.Empty;
        }

        // GET: api/CompanyHTMLReports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyHTMLReport>>> GetAllCompanyHTMLReports()
        {
            _logger.LogInformation("Getting all company HTML reports");
            return await _context.CompanyHTMLReports.ToListAsync();
        }

        // GET: api/CompanyHTMLReports/project/{projectGuid}
        [HttpGet("project/{projectGuid}")]
        public async Task<ActionResult<IEnumerable<CompanyHTMLReport>>> GetCompanyHTMLReportsByProject(string projectGuid)
        {
            try
            {
                var projectGuidObj = Guid.Parse(projectGuid);
                _logger.LogInformation($"Getting HTML reports for project: {projectGuidObj}");
                var reports = await _context.CompanyHTMLReports
                    .Where(r => r.ProjectGuid == projectGuidObj)
                    .ToListAsync();
                return Ok(reports);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, $"Invalid GUID format: {projectGuid}");
                return BadRequest("Invalid GUID format");
            }
        }

        // GET: api/CompanyHTMLReports/type/{reportTypeId}/company/{companyGuid}
        [HttpGet("type/{reportId}/project/{projectGuid}")]
        public async Task<IActionResult> GetCompanyHTMLReportsByTypeAndProject(int reportId, string projectGuid)
        {
            try
            {
                var report = await _context.CompanyHTMLReports.FirstOrDefaultAsync(r => r.ReportTypeId == reportId && r.ProjectGuid.ToString() == projectGuid);
                if (report == null)
                {
                    _logger.LogWarning($"HTML report with ID: {reportId} not found for project: {projectGuid}");
                    return NotFound();
                }

                var reportName = await _context.CompanyReportTypes
                    .Where(rt => rt.Id == report.ReportTypeId)
                    .Select(rt => rt.ReportTypeName)
                    .FirstOrDefaultAsync();

                if (report == null)
                {
                    _logger.LogWarning($"HTML report with ID: {reportId} not found");
                    return NotFound();
                }

                _logger.LogInformation($"HTML report with ID: {reportId} retrieved successfully");
                return Ok(report);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, $"Invalid GUID format: {projectGuid}");
                return BadRequest("Invalid GUID format");
            }
        }

        // GET: api/CompanyHTMLReports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyHTMLReport>> GetCompanyHTMLReport(int id)
        {
            _logger.LogInformation($"Getting HTML report with ID: {id}");

            var report = await _context.CompanyHTMLReports.FindAsync(id);

            if (report == null)
            {
                _logger.LogWarning($"HTML report with ID: {id} not found");
                return NotFound();
            }

            return report;
        }

        // PUT: api/CompanyHTMLReports/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompanyHTMLReport(int id, CompanyHTMLReport report)
        {
            if (id != report.Id)
            {
                _logger.LogWarning($"Report ID mismatch: {id} != {report.Id}");
                return BadRequest();
            }

            report.LastUpdatedDateTime = DateTime.UtcNow;
            _context.Entry(report).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"HTML report with ID: {id} updated successfully");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CompanyHTMLReportExists(id))
                {
                    _logger.LogWarning($"HTML report with ID: {id} not found during update");
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, $"Concurrency error while updating HTML report with ID: {id}");
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CompanyHTMLReports
        [HttpPost]
        public async Task<ActionResult<CompanyHTMLReport>> CreateCompanyHTMLReport(CompanyHTMLReport report)
        {
            report.LastUpdatedDateTime = DateTime.UtcNow;

            _context.CompanyHTMLReports.Add(report);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Created new HTML report with ID: {report.Id} for company: {report.CompanyGuid}");

            return CreatedAtAction(nameof(GetCompanyHTMLReport), new { id = report.Id }, report);
        }

        // DELETE: api/CompanyHTMLReports/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyHTMLReport(int id)
        {
            _logger.LogInformation($"Deleting HTML report with ID: {id}");

            var report = await _context.CompanyHTMLReports.FindAsync(id);
            if (report == null)
            {
                _logger.LogWarning($"HTML report with ID: {id} not found for deletion");
                return NotFound();
            }

            _context.CompanyHTMLReports.Remove(report);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"HTML report with ID: {id} successfully deleted");

            return NoContent();
        }

        // DELETE: delete all reports for a specific project
        [HttpDelete("project/{projectGuid}")]
        public async Task<IActionResult> DeleteAllCompanyHTMLReportsByProject(string projectGuid)
        {
            try
            {
                var projectGuidObj = Guid.Parse(projectGuid);
                _logger.LogInformation($"Deleting all HTML reports for project: {projectGuidObj}");
                var reports = await _context.CompanyHTMLReports
                    .Where(r => r.ProjectGuid == projectGuidObj)
                    .ToListAsync();
                if (!reports.Any())
                {
                    _logger.LogWarning($"No HTML reports found for project: {projectGuidObj}");
                    return NotFound("No reports found for the specified project.");
                }
                _context.CompanyHTMLReports.RemoveRange(reports);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"All HTML reports for project: {projectGuidObj} deleted successfully");
                return NoContent();
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, $"Invalid GUID format: {projectGuid}");
                return BadRequest("Invalid GUID format");
            }
        }

        // POST: api/CompanyHTMLReports/copy-templates/{companyGuid}/project/{projectGuid}
        /// <summary>
        /// Copies all active Company HTML Report Templates (ReportTypeId <= 6) for the specified company
        /// into the CompanyHTMLReports table for the given project (if they do not already exist).
        /// </summary>
        /// <param name="companyGuid">The company guid whose templates will be copied.</param>
        /// <param name="projectGuid">The target project guid.</param>
        /// <returns>List of CompanyHTMLReport records now associated with the project (after copy).</returns>
        [HttpPost("copy-templates/{companyGuid}/project/{projectGuid}")]
        public async Task<IActionResult> CopyTemplatesToProject(string companyGuid, string projectGuid)
        {
            try
            {
                if (!Guid.TryParse(companyGuid, out var companyGuidObj))
                {
                    _logger.LogWarning("Invalid companyGuid format: {CompanyGuid}", companyGuid);
                    return BadRequest("Invalid companyGuid format");
                }
                if (!Guid.TryParse(projectGuid, out var projectGuidObj))
                {
                    _logger.LogWarning("Invalid projectGuid format: {ProjectGuid}", projectGuid);
                    return BadRequest("Invalid projectGuid format");
                }

                _logger.LogInformation("Starting template copy for company {CompanyGuid} to project {ProjectGuid}", companyGuidObj, projectGuidObj);

                // Pull all templates (ReportTypeId <= 6) for the company
                var templates = await _context.CompanyHTMLReportsTemplate
                    .Where(t => t.CompanyGuid == companyGuidObj && t.ReportTypeId <= 6 && t.IsActive && t.IsGlobalTemplate)
                    .OrderBy(t => t.DisplayOrder)
                    .ToListAsync();

                if (!templates.Any())
                {
                    _logger.LogInformation("No active templates (ReportTypeId <=6) found for company {CompanyGuid}", companyGuidObj);
                    return Ok(new List<CompanyHTMLReport>()); // nothing to copy
                }

                // Get existing reports for the project to avoid duplicates
                var existingProjectReports = await _context.CompanyHTMLReports
                    .Where(r => r.ProjectGuid == projectGuidObj)
                    .Select(r => r.ReportTypeId)
                    .ToListAsync();

                var mergeUtil = new TemplateMergeUtility(_context, _logger);
                var newReports = new List<CompanyHTMLReport>();

                foreach (var template in templates)
                {
                    if (existingProjectReports.Contains(template.ReportTypeId))
                    {
                        _logger.LogDebug("Skipping template ReportTypeId {ReportTypeId} - already exists for project {ProjectGuid}", template.ReportTypeId, projectGuidObj);
                        continue; // Skip duplicates
                    }

                    // Perform token merge BEFORE creating report record
                    var mergedHtml = await mergeUtil.MergeTemplateHtmlAsync(template.ReportHTMLText, companyGuidObj, projectGuidObj);

                    var newReport = new CompanyHTMLReport
                    {
                        CompanyGuid = template.CompanyGuid,
                        ProjectGuid = projectGuidObj,
                        CompanyName = template.CompanyName,
                        ReportTypeId = template.ReportTypeId,
                        ReportHTMLText = mergedHtml,
                        IsGlobalTemplate = template.IsGlobalTemplate,
                        IsActive = template.IsActive,
                        DisplayOrder = template.DisplayOrder,
                        LastUpdatedDateTime = DateTime.UtcNow
                    };
                    newReports.Add(newReport);
                    _context.CompanyHTMLReports.Add(newReport);
                }

                if (newReports.Count == 0)
                {
                    _logger.LogInformation("No new reports created. All templates already copied for project {ProjectGuid}.", projectGuidObj);
                    // Return existing reports for the project
                    var existingReports = await _context.CompanyHTMLReports
                        .Where(r => r.ProjectGuid == projectGuidObj && r.ReportTypeId <= 6)
                        .OrderBy(r => r.DisplayOrder)
                        .ToListAsync();
                    return Ok(existingReports);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Copied {Count} templates to project {ProjectGuid}", newReports.Count, projectGuidObj);

                return Ok(newReports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error copying templates to project");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while copying templates.");
            }
        }

        private bool CompanyHTMLReportExists(int id)
        {
            return _context.CompanyHTMLReports.Any(e => e.Id == id);
        }

        private async Task<IActionResult> CreateReportPDF(string HTMLreport, string reportName)
        {
            _logger.LogInformation($"Creating PDF for report: {reportName}");

            using (var wordProcessor = new RichEditDocumentServer())
            {
                wordProcessor.HtmlText = HTMLreport;
                using (var pdfStream = new MemoryStream())
                {
                    wordProcessor.ExportToPdf(pdfStream);
                    // Return the PDF as a file from your WebAPI
                    return File(pdfStream.ToArray(), "application/pdf", $"Report_{reportName}.pdf");
                }
            }

            return Ok("PDF created successfully (placeholder)");
        }
    }
}