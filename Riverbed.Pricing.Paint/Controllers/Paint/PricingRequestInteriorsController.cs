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
    public class PricingRequestInteriorsController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public PricingRequestInteriorsController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/PricingRequestInteriors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PricingRequestInterior>>> GetPricingRequestInteriors()
        {
            return await _context.PricingRequestInteriors.ToListAsync();
        }

        // GET: api/PricingRequestInteriors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PricingRequestInterior>> GetPricingRequestInterior(int id)
        {
            var pricingRequestInterior = await _context.PricingRequestInteriors.FindAsync(id);

            if (pricingRequestInterior == null)
            {
                return NotFound();
            }

            return pricingRequestInterior;
        }

        // PUT: api/PricingRequestInteriors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPricingRequestInterior(int id, PricingRequestInterior pricingRequestInterior)
        {
            if (id != pricingRequestInterior.Id)
            {
                return BadRequest();
            }

            _context.Entry(pricingRequestInterior).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PricingRequestInteriorExists(id))
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

        // POST: api/PricingRequestInteriors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PricingRequestInterior>> PostPricingRequestInterior(PricingRequestInterior pricingRequestInterior)
        {
            _context.PricingRequestInteriors.Add(pricingRequestInterior);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPricingRequestInterior", new { id = pricingRequestInterior.Id }, pricingRequestInterior);
        }

        // DELETE: api/PricingRequestInteriors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePricingRequestInterior(int id)
        {
            var pricingRequestInterior = await _context.PricingRequestInteriors.FindAsync(id);
            if (pricingRequestInterior == null)
            {
                return NotFound();
            }

            _context.PricingRequestInteriors.Remove(pricingRequestInterior);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PricingRequestInteriorExists(int id)
        {
            return _context.PricingRequestInteriors.Any(e => e.Id == id);
        }
    }
}
