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
    public class PaintSheensController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public PaintSheensController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/PaintSheens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaintSheen>>> GetPaintSheens()
        {
            return await _context.PaintSheens.ToListAsync();
        }

        // GET: api/PaintSheens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaintSheen>> GetPaintSheen(int id)
        {
            var paintSheen = await _context.PaintSheens.FindAsync(id);

            if (paintSheen == null)
            {
                return NotFound();
            }

            return paintSheen;
        }

        // PUT: api/PaintSheens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaintSheen(int id, PaintSheen paintSheen)
        {
            if (id != paintSheen.Id)
            {
                return BadRequest();
            }

            _context.Entry(paintSheen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaintSheenExists(id))
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

        // POST: api/PaintSheens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PaintSheen>> PostPaintSheen(PaintSheen paintSheen)
        {
            _context.PaintSheens.Add(paintSheen);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPaintSheen", new { id = paintSheen.Id }, paintSheen);
        }

        // DELETE: api/PaintSheens/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaintSheen(int id)
        {
            var paintSheen = await _context.PaintSheens.FindAsync(id);
            if (paintSheen == null)
            {
                return NotFound();
            }

            _context.PaintSheens.Remove(paintSheen);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaintSheenExists(int id)
        {
            return _context.PaintSheens.Any(e => e.Id == id);
        }
    }
}
