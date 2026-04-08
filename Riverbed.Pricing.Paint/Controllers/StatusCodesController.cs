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
    public class StatusCodesController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public StatusCodesController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/StatusCodes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StatusCode>>> GetStatusCodes()
        {
            return await _context.StatusCodes.ToListAsync();
        }

        // GET: api/StatusCodes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StatusCode>> GetStatusCode(int id)
        {
            var statusCode = await _context.StatusCodes.FindAsync(id);

            if (statusCode == null)
            {
                return NotFound();
            }

            return statusCode;
        }

        // PUT: api/StatusCodes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStatusCode(int id, StatusCode statusCode)
        {
            if (id != statusCode.Id)
            {
                return BadRequest();
            }

            _context.Entry(statusCode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StatusCodeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/StatusCodes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StatusCode>> PostStatusCode(StatusCode statusCode)
        {
            _context.StatusCodes.Add(statusCode);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStatusCode", new { id = statusCode.Id }, statusCode);
        }

        // DELETE: api/StatusCodes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatusCode(int id)
        {
            var statusCode = await _context.StatusCodes.FindAsync(id);
            if (statusCode == null)
            {
                return NotFound();
            }

            _context.StatusCodes.Remove(statusCode);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StatusCodeExists(int id)
        {
            return _context.StatusCodes.Any(e => e.Id == id);
        }
    }
}
