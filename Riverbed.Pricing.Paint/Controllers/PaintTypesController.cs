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
    public class PaintTypesController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public PaintTypesController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/PaintTypes
        [HttpGet]
        public async Task<IActionResult> GetPaintTypes()
        {
            try
            {
                var paintTypes = await _context.PaintTypes.ToListAsync();
                return Ok(paintTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/PaintTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaintType>> GetPaintType(int id)
        {
            var paintType = await _context.PaintTypes.FindAsync(id);

            if (paintType == null)
            {
                return NotFound();
            }

            return paintType;
        }

        // PUT: api/PaintTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaintType(int id, PaintType paintType)
        {
            if (id != paintType.Id)
            {
                return BadRequest();
            }

            _context.Entry(paintType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaintTypeExists(id))
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

        // POST: api/PaintTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PaintType>> PostPaintType(PaintType paintType)
        {
            try
            {
                _context.PaintTypes.Add(paintType);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetPaintType", new { id = paintType.Id }, paintType);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status412PreconditionFailed, ex.Message);
            }
        }

        // DELETE: api/PaintTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaintType(int id)
        {
            var paintType = await _context.PaintTypes.FindAsync(id);
            if (paintType == null)
            {
                return NotFound();
            }

            _context.PaintTypes.Remove(paintType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaintTypeExists(int id)
        {
            return _context.PaintTypes.Any(e => e.Id == id);
        }
    }
}
