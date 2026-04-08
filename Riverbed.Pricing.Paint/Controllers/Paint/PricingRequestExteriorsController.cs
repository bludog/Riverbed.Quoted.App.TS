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
    public class PricingRequestExteriorsController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public PricingRequestExteriorsController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/PricingRequestExteriors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PricingRequestExterior>>> GetPricingRequestExteriors()
        {
            return await _context.PricingRequestExteriors.ToListAsync();
        }

        // GET: api/PricingRequestExteriors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PricingRequestExterior>> GetPricingRequestExterior(int id)
        {
            var pricingRequestExterior = await _context.PricingRequestExteriors.FindAsync(id);

            if (pricingRequestExterior == null)
            {
                return NotFound();
            }

            return pricingRequestExterior;
        }

        // PUT: api/PricingRequestExteriors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPricingRequestExterior(int id, PricingRequestExterior pricingRequestExterior)
        {
            if (id != pricingRequestExterior.Id)
            {
                return BadRequest();
            }

            _context.Entry(pricingRequestExterior).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PricingRequestExteriorExists(id))
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

        // POST: api/PricingRequestExteriors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PricingRequestExterior>> PostPricingRequestExterior(PricingRequestExterior pricingRequestExterior)
        {
            _context.PricingRequestExteriors.Add(pricingRequestExterior);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPricingRequestExterior", new { id = pricingRequestExterior.Id }, pricingRequestExterior);
        }

        // DELETE: api/PricingRequestExteriors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePricingRequestExterior(int id)
        {
            var pricingRequestExterior = await _context.PricingRequestExteriors.FindAsync(id);
            if (pricingRequestExterior == null)
            {
                return NotFound();
            }

            _context.PricingRequestExteriors.Remove(pricingRequestExterior);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PricingRequestExteriorExists(int id)
        {
            return _context.PricingRequestExteriors.Any(e => e.Id == id);
        }
    }
}
