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
    public class PricingResponseExteriorsController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public PricingResponseExteriorsController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/PricingResponseExteriors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PricingResponseExterior>>> GetPricingResponseExteriors()
        {
            return await _context.PricingResponseExteriors.ToListAsync();
        }

        // GET: api/PricingResponseExteriors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PricingResponseExterior>> GetPricingResponseExterior(int id)
        {
            var pricingResponseExterior = await _context.PricingResponseExteriors.FindAsync(id);

            if (pricingResponseExterior == null)
            {
                return NotFound();
            }

            return pricingResponseExterior;
        }

        // PUT: api/PricingResponseExteriors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPricingResponseExterior(int id, PricingResponseExterior pricingResponseExterior)
        {
            if (id != pricingResponseExterior.Id)
            {
                return BadRequest();
            }

            _context.Entry(pricingResponseExterior).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PricingResponseExteriorExists(id))
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

        // POST: api/PricingResponseExteriors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PricingResponseExterior>> PostPricingResponseExterior(PricingResponseExterior pricingResponseExterior)
        {
            _context.PricingResponseExteriors.Add(pricingResponseExterior);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPricingResponseExterior", new { id = pricingResponseExterior.Id }, pricingResponseExterior);
        }

        // DELETE: api/PricingResponseExteriors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePricingResponseExterior(int id)
        {
            var pricingResponseExterior = await _context.PricingResponseExteriors.FindAsync(id);
            if (pricingResponseExterior == null)
            {
                return NotFound();
            }

            _context.PricingResponseExteriors.Remove(pricingResponseExterior);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PricingResponseExteriorExists(int id)
        {
            return _context.PricingResponseExteriors.Any(e => e.Id == id);
        }
    }
}
