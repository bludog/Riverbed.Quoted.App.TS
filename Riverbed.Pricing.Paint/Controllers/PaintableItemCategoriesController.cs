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
    public class PaintableItemCategoriesController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public PaintableItemCategoriesController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/PaintableItemCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaintableItemCategory>>> GetPaintableItemCategories()
        {
            return await _context.PaintableItemCategories.ToListAsync();
        }

        // GET: api/PaintableItemCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaintableItemCategory>> GetPaintableItemCategory(int id)
        {
            var paintableItemCategory = await _context.PaintableItemCategories.FindAsync(id);

            if (paintableItemCategory == null)
            {
                return NotFound();
            }

            return paintableItemCategory;
        }

        // PUT: api/PaintableItemCategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaintableItemCategory(int id, PaintableItemCategory paintableItemCategory)
        {
            if (id != paintableItemCategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(paintableItemCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaintableItemCategoryExists(id))
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

        // POST: api/PaintableItemCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PaintableItemCategory>> PostPaintableItemCategory(PaintableItemCategory paintableItemCategory)
        {
            _context.PaintableItemCategories.Add(paintableItemCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPaintableItemCategory", new { id = paintableItemCategory.Id }, paintableItemCategory);
        }

        // DELETE: api/PaintableItemCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaintableItemCategory(int id)
        {
            var paintableItemCategory = await _context.PaintableItemCategories.FindAsync(id);
            if (paintableItemCategory == null)
            {
                return NotFound();
            }

            _context.PaintableItemCategories.Remove(paintableItemCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaintableItemCategoryExists(int id)
        {
            return _context.PaintableItemCategories.Any(e => e.Id == id);
        }
    }
}
