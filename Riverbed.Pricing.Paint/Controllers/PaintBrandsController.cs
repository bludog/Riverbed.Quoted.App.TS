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
    public class PaintBrandsController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public PaintBrandsController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/PaintBrands
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaintBrand>>> GetPaintBrands()
        {
            return await _context.PaintBrands.ToListAsync();
        }

        // GET: api/PaintBrands/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaintBrand>> GetPaintBrand(int id)
        {
            var paintBrand = await _context.PaintBrands.FindAsync(id);

            if (paintBrand == null)
            {
                return NotFound();
            }

            return paintBrand;
        }

        // PUT: api/PaintBrands/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaintBrand(int id, PaintBrand paintBrand)
        {
            if (id != paintBrand.Id)
            {
                return BadRequest();
            }

            _context.Entry(paintBrand).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaintBrandExists(id))
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

        // POST: api/PaintBrands
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PaintBrand>> PostPaintBrand(PaintBrand paintBrand)
        {
            _context.PaintBrands.Add(paintBrand);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPaintBrand", new { id = paintBrand.Id }, paintBrand);
        }

        // DELETE: api/PaintBrands/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaintBrand(int id)
        {
            var paintBrand = await _context.PaintBrands.FindAsync(id);
            if (paintBrand == null)
            {
                return NotFound();
            }

            _context.PaintBrands.Remove(paintBrand);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaintBrandExists(int id)
        {
            return _context.PaintBrands.Any(e => e.Id == id);
        }
    }
}
