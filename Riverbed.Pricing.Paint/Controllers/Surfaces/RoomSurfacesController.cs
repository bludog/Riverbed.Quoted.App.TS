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
    public class RoomSurfacesController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public RoomSurfacesController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/RoomSurfaces
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomSurface>>> GetRoomSurfaces()
        {
            return await _context.RoomSurfaces.ToListAsync();
        }

        // GET: api/RoomSurfaces/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomSurface>> GetRoomSurface(int id)
        {
            var roomSurface = await _context.RoomSurfaces.FindAsync(id);

            if (roomSurface == null)
            {
                return NotFound();
            }

            return roomSurface;
        }

        // PUT: api/RoomSurfaces/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomSurface(int id, RoomSurface roomSurface)
        {
            if (id != roomSurface.RoomSurfaceId)
            {
                return BadRequest();
            }

            _context.Entry(roomSurface).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomSurfaceExists(id))
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

        // POST: api/RoomSurfaces
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RoomSurface>> PostRoomSurface(RoomSurface roomSurface)
        {
            _context.RoomSurfaces.Add(roomSurface);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoomSurface", new { id = roomSurface.RoomSurfaceId }, roomSurface);
        }

        // DELETE: api/RoomSurfaces/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomSurface(int id)
        {
            var roomSurface = await _context.RoomSurfaces.FindAsync(id);
            if (roomSurface == null)
            {
                return NotFound();
            }

            _context.RoomSurfaces.Remove(roomSurface);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomSurfaceExists(int id)
        {
            return _context.RoomSurfaces.Any(e => e.RoomSurfaceId == id);
        }
    }
}
