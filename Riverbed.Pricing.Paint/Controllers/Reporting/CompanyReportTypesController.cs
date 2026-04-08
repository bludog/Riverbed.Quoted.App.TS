using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities.Reporting;

namespace Riverbed.Pricing.Paint.Controllers.Reporting
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyReportTypesController : ControllerBase
    {
        private readonly PricingDbContext _context;
        private readonly ILogger<CompanyReportTypesController> _logger;

        public CompanyReportTypesController(PricingDbContext context, ILogger<CompanyReportTypesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/CompanyReportTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyReportType>>> GetAllCompanyReportTypes()
        {
            _logger.LogInformation("Getting all company report types");
            var reportTypes = await _context.CompanyReportTypes.ToListAsync();
            return reportTypes;
        }

        // GET: api/CompanyReportTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyReportType>> GetCompanyReportType(int id)
        {
            _logger.LogInformation($"Getting report type with ID: {id}");
            
            var reportType = await _context.CompanyReportTypes.FindAsync(id);

            if (reportType == null)
            {
                _logger.LogWarning($"Report type with ID: {id} not found");
                return NotFound();
            }

            return reportType;
        }

        // PUT: api/CompanyReportTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompanyReportType(int id, CompanyReportType reportType)
        {
            if (id != reportType.Id)
            {
                _logger.LogWarning($"Report type ID mismatch: {id} != {reportType.Id}");
                return BadRequest();
            }

            _context.Entry(reportType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Report type with ID: {id} updated successfully");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CompanyReportTypeExists(id))
                {
                    _logger.LogWarning($"Report type with ID: {id} not found during update");
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, $"Concurrency error while updating report type with ID: {id}");
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CompanyReportTypes
        [HttpPost]
        public async Task<ActionResult<CompanyReportType>> CreateCompanyReportType(CompanyReportType reportType)
        {
            _context.CompanyReportTypes.Add(reportType);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Created new report type with ID: {reportType.Id}");

            return CreatedAtAction(nameof(GetCompanyReportType), new { id = reportType.Id }, reportType);
        }

        // DELETE: api/CompanyReportTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyReportType(int id)
        {
            _logger.LogInformation($"Deleting report type with ID: {id}");
            
            var reportType = await _context.CompanyReportTypes.FindAsync(id);
            if (reportType == null)
            {
                _logger.LogWarning($"Report type with ID: {id} not found for deletion");
                return NotFound();
            }

            _context.CompanyReportTypes.Remove(reportType);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Report type with ID: {id} successfully deleted");

            return NoContent();
        }

        private bool CompanyReportTypeExists(int id)
        {
            return _context.CompanyReportTypes.Any(e => e.Id == id);
        }
    }
}