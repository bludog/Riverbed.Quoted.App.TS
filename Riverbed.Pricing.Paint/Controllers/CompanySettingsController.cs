using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;

namespace Riverbed.Pricing.Paint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanySettingsController : ControllerBase
    {
        private readonly PricingDbContext _context;
        private readonly ILogger<CompanySettingsController> _logger;

        public CompanySettingsController(PricingDbContext context, ILogger<CompanySettingsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/CompanySettings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanySettings>>> GetCompanySettings()
        {
            try
            {
                _logger.LogInformation("Getting all company settings");
                return await _context.CompanySettings.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting company settings");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting company settings.");
            }
        }

        // GET: api/CompanySettings/5
        [HttpGet("{companyId}")]
        public async Task<ActionResult<CompanySettings>> GetCompanySettings(string companyId)
        {
            try
            {
                _logger.LogInformation($"Getting company settings for company ID: {companyId}");
                var compId = Guid.Parse(companyId);
                var companySettings = await _context.CompanySettings.FirstOrDefaultAsync(s => s.CompanyId.Equals(compId));

                if (companySettings == null)
                {
                    _logger.LogWarning($"Company settings for company ID: {companyId} not found");
                    return NotFound();
                }

                return Ok(companySettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting company settings for company ID: {companyId}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting company settings.");
            }
        }

        // PUT: api/CompanySettings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompanySettings(int id, CompanySettings companySettings)
        {
            if (id != companySettings.Id)
            {
                _logger.LogWarning($"Company settings ID mismatch: {id} != {companySettings.Id}");
                return BadRequest();
            }

            _context.Entry(companySettings).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Company settings with ID: {id} updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanySettingsExists(id))
                {
                    _logger.LogWarning($"Company settings with ID: {id} not found during update");
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"Concurrency error while updating company settings with ID: {id}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating company settings with ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating company settings.");
            }

            return NoContent();
        }

        // POST: api/CompanySettings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CompanySettings>> PostCompanySettings(CompanySettings companySettings)
        {
            try
            {
                _context.CompanySettings.Add(companySettings);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Company settings with ID: {companySettings.Id} created successfully");
                return CreatedAtAction("GetCompanySettings", new { id = companySettings.Id }, companySettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the company settings");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the company settings.");
            }
        }

        // DELETE: api/CompanySettings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanySettings(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting company settings with ID: {id}");
                var companySettings = await _context.CompanySettings.FindAsync(id);
                if (companySettings == null)
                {
                    _logger.LogWarning($"Company settings with ID: {id} not found");
                    return NotFound();
                }

                _context.CompanySettings.Remove(companySettings);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Company settings with ID: {id} deleted successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting company settings with ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the company settings.");
            }
        }

        private bool CompanySettingsExists(int id)
        {
            return _context.CompanySettings.Any(e => e.Id == id);
        }
    }
}
