using DevExpress.XtraReports;
using DevExpress.XtraRichEdit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Controllers.Reporting
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyHTMLReportTemplateController : ControllerBase
    {
        private readonly PricingDbContext _context;
        private readonly ILogger<CompanyHTMLReportTemplateController> _logger;

        public CompanyHTMLReportTemplateController(PricingDbContext context, ILogger<CompanyHTMLReportTemplateController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/CompanyHTMLReports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyHTMLReportTemplate>>> GetAllCompanyHTMLReports()
        {
            _logger.LogInformation("Getting all company HTML reports");
            return await _context.CompanyHTMLReportsTemplate.ToListAsync();
        }

        // GET: api/CompanyHTMLReports/company/{companyGuid}
        [HttpGet("company/{companyGuid}")]
        public async Task<ActionResult<IEnumerable<CompanyHTMLReportTemplate>>> GetCompanyHTMLReportsByCompany(string companyGuid)
        {
            try
            {
                var companyGuidObj = Guid.Parse(companyGuid);
                _logger.LogInformation($"Getting HTML reports for company: {companyGuidObj}");
                
                var reports = await _context.CompanyHTMLReportsTemplate
                    .Where(r => r.CompanyGuid == companyGuidObj)
                    .OrderBy(r => r.DisplayOrder)
                    .ToListAsync();

                // Fallback: if no templates exist for this company, clone global templates from the predefined source company
                if (!reports.Any())
                {
                    reports = await CopyDefaultTeplatesIntoCustomer(companyGuidObj);
                }
                return reports;
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, $"Invalid GUID format: {companyGuid}");
                return BadRequest("Invalid GUID format");
            }            
        }

        async Task<List<CompanyHTMLReportTemplate>> CopyDefaultTeplatesIntoCustomer(Guid companyGuidObj)
        {
            var globalSourceCompanyGuid = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            _logger.LogInformation("No templates found for company {CompanyGuid}. Attempting to clone global templates from {GlobalSource}", companyGuidObj, globalSourceCompanyGuid);

            var reports = new List<CompanyHTMLReportTemplate>();

            var globalTemplates = await _context.CompanyHTMLReportsTemplate
                .Where(t => t.CompanyGuid == globalSourceCompanyGuid && t.IsGlobalTemplate && t.IsActive)
                .OrderBy(t => t.DisplayOrder)
                .ToListAsync();

            if (globalTemplates.Any())
            {
                foreach (var g in globalTemplates)
                {
                    var clone = new CompanyHTMLReportTemplate
                    {
                        CompanyGuid = companyGuidObj,
                        CompanyName = g.CompanyName,
                        ReportTypeId = g.ReportTypeId,
                        ReportName = g.ReportName,
                        ReportHTMLText = g.ReportHTMLText,
                        IsActive = g.IsActive,
                        IsGlobalTemplate = g.IsGlobalTemplate,
                        DisplayOrder = g.DisplayOrder,
                        LastUpdatedDateTime = DateTime.UtcNow
                    };
                    _context.CompanyHTMLReportsTemplate.Add(clone);
                }
                await _context.SaveChangesAsync();
                reports = await _context.CompanyHTMLReportsTemplate
                    .Where(r => r.CompanyGuid == companyGuidObj)
                    .OrderBy(r => r.DisplayOrder)
                    .ToListAsync();
                _logger.LogInformation("Cloned {Count} global templates to company {CompanyGuid}", reports.Count, companyGuidObj);
            }
            else
            {
                _logger.LogInformation("No global templates available to clone from {GlobalSource}", globalSourceCompanyGuid);
                // Return empty list (still OK)
            }

            return reports;
        }

        // GET: api/CompanyHTMLReports/type/{reportTypeId}/company/{companyGuid}
        [HttpGet("type/{reportTypeId}/company/{companyGuid}")]
        public async Task<IActionResult> GetCompanyHTMLReportsByTypeAndCompany(int reportTypeId, string companyGuid)
        {
            try
            {
                var companyGuidObj = Guid.Parse(companyGuid);
                var report = await _context.CompanyHTMLReportsTemplate.FirstOrDefaultAsync(r => r.ReportTypeId == reportTypeId && r.CompanyGuid == companyGuidObj);
                if(report == null)
                    {
                    _logger.LogWarning($"HTML report with ID: {reportTypeId} for company: {companyGuid} not found");
                    return NotFound();
                }
                var reportName = await _context.CompanyReportTypes
                    .Where(rt => rt.Id == report.ReportTypeId)
                    .Select(rt => rt.ReportTypeName)
                    .FirstOrDefaultAsync();

                if (report == null)
                {
                    _logger.LogWarning($"HTML report with ID: {reportTypeId} not found");
                    return NotFound();
                }

                _logger.LogInformation($"HTML report with ID: {reportTypeId} retrieved successfully");
                //  var reportAsPDF = await CreateReportPDF(report.ReportHTMLText, reportName); // Call to create PDF from HTML content
                // Return the CompanyHTMLReport object directly
                return Ok(report);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, $"Invalid GUID format: {companyGuid}");
                return BadRequest("Invalid GUID format");
            }
        }

        // NEW: Active templates minimal endpoint
        // GET: api/CompanyHTMLReportTemplate/templates/active/{companyGuid}
        [HttpGet("templates/active/{companyGuid}")]
        public async Task<ActionResult<IEnumerable<CompanyHTMLReportsTemplateMinimal>>> GetActiveTemplatesMinimal(string companyGuid)
        {
            try
            {
                var companyGuidObj = Guid.Parse(companyGuid);
                _logger.LogInformation("Getting active minimal templates for company: {CompanyGuid}", companyGuidObj);

                var templates = await _context.CompanyHTMLReportsTemplate
                    .Where(r => r.CompanyGuid == companyGuidObj && r.IsActive)
                    .OrderBy(r => r.DisplayOrder)
                    .Select(t => new CompanyHTMLReportsTemplateMinimal
                    {
                        Id = t.Id,
                        ReportName = t.ReportName,
                        ReportTypeId = t.ReportTypeId,
                        DisplayOrder = t.DisplayOrder
                    })
                    .ToListAsync();

                return Ok(templates);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid GUID format: {CompanyGuid}", companyGuid);
                return BadRequest("Invalid GUID format");
            }
        }

        // GET: api/CompanyHTMLReports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyHTMLReportTemplate>> GetCompanyHTMLReport(int id)
        {
            _logger.LogInformation($"Getting HTML report with ID: {id}");
            
            var report = await _context.CompanyHTMLReportsTemplate.FindAsync(id);

            if (report == null)
            {
                _logger.LogWarning($"HTML report with ID: {id} not found");
                return NotFound();
            }

            return report;
        }

        // PUT: api/CompanyHTMLReports/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompanyHTMLReport(int id, CompanyHTMLReportTemplate report)
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
        public async Task<ActionResult<CompanyHTMLReportTemplate>> CreateCompanyHTMLReport(CompanyHTMLReportTemplate report)
        {
            // Set the last update date to current UTC time
            report.LastUpdatedDateTime = DateTime.UtcNow;
            
            _context.CompanyHTMLReportsTemplate.Add(report);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Created new HTML report with ID: {report.Id} for company: {report.CompanyGuid}");

            return CreatedAtAction(nameof(GetCompanyHTMLReport), new { id = report.Id }, report);
        }

        // DELETE: api/CompanyHTMLReports/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyHTMLReport(int id)
        {
            _logger.LogInformation($"Deleting HTML report with ID: {id}");
            
            var report = await _context.CompanyHTMLReportsTemplate.FindAsync(id);
            if (report == null)
            {
                _logger.LogWarning($"HTML report with ID: {id} not found for deletion");
                return NotFound();
            }

            _context.CompanyHTMLReportsTemplate.Remove(report);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"HTML report with ID: {id} successfully deleted");

            return NoContent();
        }

        // GET: api/CompanyHTMLReports/templates/minimal/global/{companyGuid}
        // Returns minimal info (Id, ReportName, ReportTypeId, DisplayOrder) for active global templates for the specified company
        [HttpGet("templates/minimal/global/{companyGuid}")]
        public async Task<ActionResult<IEnumerable<CompanyHTMLReportsTemplateMinimal>>> GetMinimalGlobalTemplateInfo(string companyGuid)
        {
            return await GetMinimatTeplateList(companyGuid, true);
        }

        // GET: api/CompanyHTMLReports/templates/minimal/{companyGuid}
        [HttpGet("templates/minimal/{companyGuid}")]
        public async Task<ActionResult<IEnumerable<CompanyHTMLReportsTemplateMinimal>>> GetMinimalTemplateInfo(string companyGuid)
        {
            return await GetMinimatTeplateList(companyGuid, false);
        }

        async Task<ActionResult<IEnumerable<CompanyHTMLReportsTemplateMinimal>>> GetMinimatTeplateList(string companyGuid, bool isGlobal = false)
        {
            try
            {
                var companyGuidObj = Guid.Parse(companyGuid);
                _logger.LogInformation($"Getting minimal template info for company: {companyGuidObj}");

                var templates = await _context.CompanyHTMLReportsTemplate
                    .Where(r => r.CompanyGuid == companyGuidObj && r.IsActive == true && r.IsGlobalTemplate == isGlobal)
                    .Select(t => new CompanyHTMLReportsTemplateMinimal
                    {
                        Id = t.Id,
                        ReportName = t.ReportName,
                        ReportTypeId = t.ReportTypeId,
                        DisplayOrder = t.DisplayOrder
                    })
                    .ToListAsync();

                // Fallback: if no templates exist for this company, clone global templates from the predefined source company
                if (!templates.Any())
                {
                    var companyTemplates = await CopyDefaultTeplatesIntoCustomer(companyGuidObj);
                    templates = companyTemplates.Select(t => new CompanyHTMLReportsTemplateMinimal
                    {
                        Id = t.Id,
                        ReportName = t.ReportName,
                        ReportTypeId = t.ReportTypeId,
                        DisplayOrder = t.DisplayOrder
                    }).ToList();
                }

                return Ok(templates);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, $"Invalid GUID format: {companyGuid}");
                return BadRequest("Invalid GUID format");
            }
        }

        private bool CompanyHTMLReportExists(int id)
        {
            return _context.CompanyHTMLReportsTemplate.Any(e => e.Id == id);
        }

        // Create a new report PDF from HTML content
        private async Task<IActionResult> CreateReportPDF(string HTMLreport, string reportName)
        {
            // This method would contain logic to convert HTML to PDF
            // For now, we will just log the action and return a placeholder response
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