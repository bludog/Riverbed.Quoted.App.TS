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
    public class GlobalDefaultsController : ControllerBase
    {
        private readonly PricingDbContext _context;
        private readonly ILogger<GlobalDefaultsController> _logger;

        public GlobalDefaultsController(PricingDbContext context, ILogger<GlobalDefaultsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/GlobalDefaults
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GlobalDefaults>>> GetGlobalDefaults()
        {
            _logger.LogInformation("Getting all global defaults");
            return await _context.GlobalDefaults.ToListAsync();
        }

        // GET: api/GlobalDefaults/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GlobalDefaults>> GetGlobalDefaults(int id)
        {
            _logger.LogInformation($"Getting global defaults with ID: {id}");
            var globalDefaults = await _context.GlobalDefaults.FindAsync(id);

            if (globalDefaults == null)
            {
                _logger.LogWarning($"Global defaults with ID: {id} not found");
                return NotFound();
            }

            return globalDefaults;
        }

        // PUT: api/GlobalDefaults/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGlobalDefaults(int id, GlobalDefaults globalDefaults)
        {
            if (id != globalDefaults.Id)
            {
                _logger.LogWarning($"Global defaults ID mismatch: {id} != {globalDefaults.Id}");
                return BadRequest();
            }

            _context.Entry(globalDefaults).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Global defaults with ID: {id} updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GlobalDefaultsExists(id))
                {
                    _logger.LogWarning($"Global defaults with ID: {id} not found during update");
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"Concurrency error while updating global defaults with ID: {id}");
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/GlobalDefaults
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GlobalDefaults>> PostGlobalDefaults(GlobalDefaults globalDefaults)
        {
            try
            {
                _context.GlobalDefaults.Add(globalDefaults);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Global defaults with ID: {globalDefaults.Id} created successfully");
                return CreatedAtAction("GetGlobalDefaults", new { id = globalDefaults.Id }, globalDefaults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the global defaults");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the global defaults.");
            }
        }

        // DELETE: api/GlobalDefaults/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGlobalDefaults(int id)
        {
            _logger.LogInformation($"Deleting global defaults with ID: {id}");
            var globalDefaults = await _context.GlobalDefaults.FindAsync(id);
            if (globalDefaults == null)
            {
                _logger.LogWarning($"Global defaults with ID: {id} not found");
                return NotFound();
            }

            _context.GlobalDefaults.Remove(globalDefaults);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Global defaults with ID: {id} deleted successfully");
            return NoContent();
        }

        private bool GlobalDefaultsExists(int id)
        {
            return _context.GlobalDefaults.Any(e => e.Id == id);
        }
    }
}
