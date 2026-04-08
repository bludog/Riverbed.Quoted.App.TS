using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;

namespace Riverbed.Pricing.Paint.Controllers_Surfaces
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurfaceTypeLookupsController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public SurfaceTypeLookupsController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/SurfaceTypeLookups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurfaceTypeLookup>>> GetSurfaceTypes()
        {
            return await _context.SurfaceTypes.ToListAsync();
        }

        // GET: api/SurfaceTypeLookups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SurfaceTypeLookup>> GetSurfaceTypeLookup(byte id)
        {
            var surfaceTypeLookup = await _context.SurfaceTypes.FindAsync(id);

            if (surfaceTypeLookup == null)
            {
                return NotFound();
            }

            return surfaceTypeLookup;
        }

        // PUT: api/SurfaceTypeLookups/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSurfaceTypeLookup(byte id, SurfaceTypeLookup surfaceTypeLookup)
        {
            if (id != surfaceTypeLookup.SurfaceTypeId)
            {
                return BadRequest();
            }

            _context.Entry(surfaceTypeLookup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SurfaceTypeLookupExists(id))
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

        // POST: api/SurfaceTypeLookups
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SurfaceTypeLookup>> PostSurfaceTypeLookup(SurfaceTypeLookup surfaceTypeLookup)
        {
            _context.SurfaceTypes.Add(surfaceTypeLookup);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SurfaceTypeLookupExists(surfaceTypeLookup.SurfaceTypeId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSurfaceTypeLookup", new { id = surfaceTypeLookup.SurfaceTypeId }, surfaceTypeLookup);
        }

        // DELETE: api/SurfaceTypeLookups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurfaceTypeLookup(byte id)
        {
            var surfaceTypeLookup = await _context.SurfaceTypes.FindAsync(id);
            if (surfaceTypeLookup == null)
            {
                return NotFound();
            }

            _context.SurfaceTypes.Remove(surfaceTypeLookup);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SurfaceTypeLookupExists(byte id)
        {
            return _context.SurfaceTypes.Any(e => e.SurfaceTypeId == id);
        }
    }
}
