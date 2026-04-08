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
    public class PricingTypesController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public PricingTypesController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/PricingTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PricingType>>> GetPricingType()
        {
            return await _context.PricingType.ToListAsync();
        }

        // GET: api/PricingTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PricingType>> GetPricingType(int id)
        {
            var pricingType = await _context.PricingType.FindAsync(id);

            if (pricingType == null)
            {
                return NotFound();
            }

            return pricingType;
        }

        // PUT: api/PricingTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPricingType(int id, PricingType pricingType)
        {
            if (id != pricingType.Id)
            {
                return BadRequest();
            }

            _context.Entry(pricingType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PricingTypeExists(id))
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

        // POST: api/PricingTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PricingType>> PostPricingType(PricingType pricingType)
        {
            _context.PricingType.Add(pricingType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPricingType", new { id = pricingType.Id }, pricingType);
        }

        // DELETE: api/PricingTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePricingType(int id)
        {
            var pricingType = await _context.PricingType.FindAsync(id);
            if (pricingType == null)
            {
                return NotFound();
            }

            _context.PricingType.Remove(pricingType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PricingTypeExists(int id)
        {
            return _context.PricingType.Any(e => e.Id == id);
        }
    }
}
