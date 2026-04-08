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
    public class AreaItemsController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public AreaItemsController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/AreaItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AreaItem>>> GetAreaItems()
        {
            return await _context.AreaItems.ToListAsync();
        }

        // GET: api/AreaItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AreaItem>> GetAreaItem(int id)
        {
            var areaItem = await _context.AreaItems.FindAsync(id);

            if (areaItem == null)
            {
                return NotFound();
            }

            return areaItem;
        }

        // PUT: api/AreaItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAreaItem(int id, AreaItem areaItem)
        {
            if (id != areaItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(areaItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AreaItemExists(id))
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

        // POST: api/AreaItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AreaItem>> PostAreaItem(AreaItem areaItem)
        {
            _context.AreaItems.Add(areaItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAreaItem", new { id = areaItem.Id }, areaItem);
        }

        // DELETE: api/AreaItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAreaItem(int id)
        {
            var areaItem = await _context.AreaItems.FindAsync(id);
            if (areaItem == null)
            {
                return NotFound();
            }

            _context.AreaItems.Remove(areaItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AreaItemExists(int id)
        {
            return _context.AreaItems.Any(e => e.Id == id);
        }
    }
}
