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
    public class PaintableItemsController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public PaintableItemsController(PricingDbContext context)
        {
            _context = context;
        }

     
        // GET: api/PaintableItems
        [HttpGet("Room/{RoomId}")]
        public async Task<ActionResult<IEnumerable<PaintableItem>>> GetPaintableItems(int RoomId)
        {
            if (RoomId <= 0)
            {
                return Ok(new List<PaintableItem>()); // Return an empty list
            }

            return await _context.PaintableItems.Where(pi => pi.RoomId == RoomId).ToListAsync();
        }

        // GET: api/PaintableItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaintableItem>> GetPaintableItem(int id)
        {
            var paintableItem = await _context.PaintableItems.FindAsync(id);

            if (paintableItem == null)
            {
                return NotFound();
            }

            return paintableItem;
        }

        // PUT: api/PaintableItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaintableItem(int id, PaintableItem paintableItem)
        {
            if (id != paintableItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(paintableItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaintableItemExists(id))
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

        // POST: api/PaintableItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PaintableItem>> PostPaintableItem(PaintableItem paintableItem)
        {
            _context.PaintableItems.Add(paintableItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPaintableItem", new { id = paintableItem.Id }, paintableItem);
        }

        // DELETE: api/PaintableItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaintableItem(int id)
        {
            var paintableItem = await _context.PaintableItems.FindAsync(id);
            if (paintableItem == null)
            {
                return NotFound();
            }

            _context.PaintableItems.Remove(paintableItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaintableItemExists(int id)
        {
            return _context.PaintableItems.Any(e => e.Id == id);
        }
    }
}
