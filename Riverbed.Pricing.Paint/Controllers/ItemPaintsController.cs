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
    public class ItemPaintsController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public ItemPaintsController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/ItemPaints
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemPaint>>> GetItemPaints()
        {
            return await _context.ItemPaints.ToListAsync();
        }

        // GET: api/ItemPaints/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemPaint>> GetItemPaint(int id)
        {
            var itemPaint = await _context.ItemPaints.FindAsync(id);

            if (itemPaint == null)
            {
                return NotFound();
            }

            return itemPaint;
        }

        // PUT: api/ItemPaints/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItemPaint(int id, ItemPaint itemPaint)
        {
            if (id != itemPaint.Id)
            {
                return BadRequest();
            }

            _context.Entry(itemPaint).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemPaintExists(id))
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

        // POST: api/ItemPaints
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ItemPaint>> PostItemPaint(ItemPaint itemPaint)
        {
            _context.ItemPaints.Add(itemPaint);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItemPaint", new { id = itemPaint.Id }, itemPaint);
        }

        // DELETE: api/ItemPaints/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemPaint(int id)
        {
            var itemPaint = await _context.ItemPaints.FindAsync(id);
            if (itemPaint == null)
            {
                return NotFound();
            }

            _context.ItemPaints.Remove(itemPaint);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemPaintExists(int id)
        {
            return _context.ItemPaints.Any(e => e.Id == id);
        }
    }
}
