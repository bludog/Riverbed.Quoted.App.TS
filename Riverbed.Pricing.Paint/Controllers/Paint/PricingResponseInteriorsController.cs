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
    public class PricingResponseInteriorsController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public PricingResponseInteriorsController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/PricingResponseInteriors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PricingResponseInterior>>> GetPricingResponseInteriors()
        {
            return await _context.PricingResponseInteriors.ToListAsync();
        }

        // GET: api/PricingResponseInteriors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PricingResponseInterior>> GetPricingResponseInterior(int id)
        {
            var pricingResponseInterior = await _context.PricingResponseInteriors.FindAsync(id);

            if (pricingResponseInterior == null)
            {
                return NotFound();
            }

            return pricingResponseInterior;
        }

        // PUT: api/PricingResponseInteriors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPricingResponseInterior(int id, PricingResponseInterior pricingResponseInterior)
        {
            if (id != pricingResponseInterior.Id)
            {
                return BadRequest();
            }

            _context.Entry(pricingResponseInterior).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PricingResponseInteriorExists(id))
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

        // POST: api/PricingResponseInteriors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PricingResponseInterior>> PostPricingResponseInterior(PricingResponseInterior pricingResponseInterior)
        {
            _context.PricingResponseInteriors.Add(pricingResponseInterior);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPricingResponseInterior", new { id = pricingResponseInterior.Id }, pricingResponseInterior);
        }

        // DELETE: api/PricingResponseInteriors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePricingResponseInterior(int id)
        {
            var pricingResponseInterior = await _context.PricingResponseInteriors.FindAsync(id);
            if (pricingResponseInterior == null)
            {
                return NotFound();
            }

            _context.PricingResponseInteriors.Remove(pricingResponseInterior);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PricingResponseInteriorExists(int id)
        {
            return _context.PricingResponseInteriors.Any(e => e.Id == id);
        }
    }
}
