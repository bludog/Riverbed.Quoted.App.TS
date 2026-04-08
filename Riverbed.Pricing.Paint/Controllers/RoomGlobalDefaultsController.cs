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
    public class RoomGlobalDefaultsController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public RoomGlobalDefaultsController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/RoomGlobalDefaults
        [HttpGet]
        public async Task<ActionResult<RoomGlobalDefaults>> GetRoomGlobalDefaults()
        {
            return await _context.RoomGlobalDefaults.FindAsync(3);
        }

        // GET: api/RoomGlobalDefaults/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomGlobalDefaults>> GetRoomGlobalDefaults(int id)
        {
            var roomGlobalDefaults = await _context.RoomGlobalDefaults.FindAsync(id);

            if (roomGlobalDefaults == null)
            {
                return NotFound();
            }

            return roomGlobalDefaults;
        }

        // PUT: api/RoomGlobalDefaults/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomGlobalDefaults(int id, RoomGlobalDefaults roomGlobalDefaults)
        {
            if (id != roomGlobalDefaults.Id)
            {
                return BadRequest();
            }

            _context.Entry(roomGlobalDefaults).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomGlobalDefaultsExists(id))
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

        // POST: api/RoomGlobalDefaults
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RoomGlobalDefaults>> PostRoomGlobalDefaults(RoomGlobalDefaults roomGlobalDefaults)
        {
            _context.RoomGlobalDefaults.Add(roomGlobalDefaults);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoomGlobalDefaults", new { id = roomGlobalDefaults.Id }, roomGlobalDefaults);
        }

        // DELETE: api/RoomGlobalDefaults/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomGlobalDefaults(int id)
        {
            var roomGlobalDefaults = await _context.RoomGlobalDefaults.FindAsync(id);
            if (roomGlobalDefaults == null)
            {
                return NotFound();
            }

            _context.RoomGlobalDefaults.Remove(roomGlobalDefaults);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomGlobalDefaultsExists(int id)
        {
            return _context.RoomGlobalDefaults.Any(e => e.Id == id);
        }
    }
}
