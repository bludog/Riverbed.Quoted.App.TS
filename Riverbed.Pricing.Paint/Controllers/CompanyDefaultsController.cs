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
    public class CompanyDefaultsController : ControllerBase
    {
        private readonly PricingDbContext _context;
        private readonly ILogger<CompanyDefaultsController> _logger;

        public CompanyDefaultsController(PricingDbContext context, ILogger<CompanyDefaultsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/CompanyDefaults
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyDefaults>>> GetCompanyDefaults()
        {
            try
            {
                _logger.LogInformation("Getting all company defaults");
                return await _context.CompanyDefaults.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting company defaults");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting company defaults.");
            }
        }

        // GET: api/CompanyDefaults/5
        [HttpGet("{companyId}")]
        public async Task<ActionResult<CompanyDefaults>> GetCompanyDefaults(string companyId)
        {
            try
            {
                _logger.LogInformation($"Getting company defaults for company ID: {companyId}");
                var companyDefaults = await _context.CompanyDefaults.FirstOrDefaultAsync(d => d.CompanyId.Equals(Guid.Parse(companyId)));
                _logger.LogInformation($"Got Company defaults: {companyDefaults}");

                if (companyDefaults == null)
                {
                    _logger.LogWarning($"Company defaults for company ID: {companyId} not found");
                    return NotFound();
                }

                return Ok(companyDefaults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting company defaults for company ID: {companyId}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting company defaults.");
            }
        }

        // PUT: api/CompanyDefaults/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompanyDefaults(int id, CompanyDefaults companyDefaults)
        {
            if (id != companyDefaults.Id)
            {
                _logger.LogWarning($"Company defaults ID mismatch: {id} != {companyDefaults.Id}");
                return BadRequest();
            }

            _context.Entry(companyDefaults).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Company defaults with ID: {id} updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyDefaultsExists(id))
                {
                    _logger.LogWarning($"Company defaults with ID: {id} not found during update");
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"Concurrency error while updating company defaults with ID: {id}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating company defaults with ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating company defaults.");
            }

            return NoContent();
        }

        // POST: api/CompanyDefaults
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CompanyDefaults>> PostCompanyDefaults(CompanyDefaults companyDefaults)
        {
            try
            {
                _context.CompanyDefaults.Add(companyDefaults);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Company defaults with ID: {companyDefaults.Id} created successfully");
                return CreatedAtAction("GetCompanyDefaults", new { id = companyDefaults.Id }, companyDefaults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the company defaults");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the company defaults.");
            }
        }

        // DELETE: api/CompanyDefaults/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyDefaults(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting company defaults with ID: {id}");
                var companyDefaults = await _context.CompanyDefaults.FindAsync(id);
                if (companyDefaults == null)
                {
                    _logger.LogWarning($"Company defaults with ID: {id} not found");
                    return NotFound();
                }

                _context.CompanyDefaults.Remove(companyDefaults);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Company defaults with ID: {id} deleted successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting company defaults with ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the company defaults.");
            }
        }

        private bool CompanyDefaultsExists(int id)
        {
            return _context.CompanyDefaults.Any(e => e.Id == id);
        }
    }
}
