using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Shared;
using Riverbed.Pricing.Paint.Shared.Data;

namespace Riverbed.Pricing.Paint.Controllers.Paint
{
    [Route("api/[controller]")]
    [ApiController]
    public class PricingInteriorDefaultsController : ControllerBase
    {
        private readonly PricingDbContext _context;
        private readonly ILogger<PricingInteriorDefaultsController> _logger;

        public PricingInteriorDefaultsController(PricingDbContext context, ILogger<PricingInteriorDefaultsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/PricingInteriorDefaults
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PricingInteriorDefault>>> GetPricingInteriorDefaults()
        {
            try
            {
                _logger.LogInformation("Getting all pricing interior defaults");
                return await _context.PricingInteriorDefaults.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting pricing interior defaults");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting pricing interior defaults.");
            }
        }

        // GET: api/PricingInteriorDefaults/5
        [HttpGet("{guidId}")]
        public async Task<ActionResult<PricingInteriorDefault>> GetPricingInteriorDefault(Guid guidId)
        {
            try
            {
                _logger.LogInformation($"Getting pricing interior default with CompanyId: {guidId}");
                var pricingInteriorDefault = await _context.PricingInteriorDefaults.FirstOrDefaultAsync(d => d.CompanyId == guidId);

                if (pricingInteriorDefault == null)
                {
                    _logger.LogWarning($"Pricing interior default with CompanyId: {guidId} not found");
                    return NotFound();
                }

                return pricingInteriorDefault;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting pricing interior default with CompanyId: {guidId}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while getting pricing interior default.");
            }
        }

        // PUT: api/PricingInteriorDefaults/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{guidId}")]
        public async Task<IActionResult> PutPricingInteriorDefault(Guid guidId, PricingInteriorDefault pricingInteriorDefault)
        {
            if (guidId != pricingInteriorDefault.CompanyId)
            {
                _logger.LogWarning($"Pricing interior default CompanyId mismatch: {guidId} != {pricingInteriorDefault.CompanyId}");
                return BadRequest();
            }

            _context.Entry(pricingInteriorDefault).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Pricing interior default with CompanyId: {guidId} updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PricingInteriorDefaultExists(guidId))
                {
                    _logger.LogWarning($"Pricing interior default with CompanyId: {guidId} not found during update");
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"Concurrency error while updating pricing interior default with CompanyId: {guidId}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating pricing interior default with CompanyId: {guidId}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating pricing interior default.");
            }

            return NoContent();
        }

        // POST: api/PricingInteriorDefaults
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PricingInteriorDefault>> PostPricingInteriorDefault(PricingInteriorDefault pricingInteriorDefault)
        {
            try
            {
                _context.PricingInteriorDefaults.Add(pricingInteriorDefault);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Pricing interior default with CompanyId: {pricingInteriorDefault.CompanyId} created successfully");
                return CreatedAtAction("GetPricingInteriorDefault", new { guidId = pricingInteriorDefault.CompanyId }, pricingInteriorDefault);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the pricing interior default");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the pricing interior default.");
            }
        }

        // DELETE: api/PricingInteriorDefaults/5
        [HttpDelete("{guidId}")]
        public async Task<IActionResult> DeletePricingInteriorDefault(Guid guidId)
        {
            try
            {
                _logger.LogInformation($"Deleting pricing interior default with CompanyId: {guidId}");
                var pricingInteriorDefault = await _context.PricingInteriorDefaults.FirstOrDefaultAsync(d => d.CompanyId == guidId);
                if (pricingInteriorDefault == null)
                {
                    _logger.LogWarning($"Pricing interior default with CompanyId: {guidId} not found");
                    return NotFound();
                }

                _context.PricingInteriorDefaults.Remove(pricingInteriorDefault);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Pricing interior default with CompanyId: {guidId} deleted successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting pricing interior default with CompanyId: {guidId}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the pricing interior default.");
            }
        }

        private bool PricingInteriorDefaultExists(Guid guidId)
        {
            return _context.PricingInteriorDefaults.Any(e => e.CompanyId == guidId);
        }
    }
}
