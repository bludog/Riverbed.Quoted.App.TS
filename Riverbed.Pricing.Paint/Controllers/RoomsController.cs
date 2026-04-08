using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nextended.Core.Extensions;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;
using Microsoft.Data.SqlClient;

namespace Riverbed.Pricing.Paint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public RoomsController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return room;
        }

        // GET: api/Rooms/ByProjectId/5
        [HttpGet("ByProjectId/{projectId}")]
        public async Task<ActionResult<List<Room>>> GetRoomByProjectId(int projectId)
        {
            // Fetch Room entities including paintable items
            var entities = await _context.Rooms
                .Where(r => r.ProjectDataId == projectId)
                .Include(r => r.PaintableItems)
                .ToListAsync();

            if (entities == null || entities.Count == 0)
                return Ok(new List<Room>());

            // Get the project to retrieve company information
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project != null)
            {
                // Get company customer to find the company ID
                var companyCustomer = await _context.CompanyCustomers.FirstOrDefaultAsync(cc => cc.Id == project.CompanyCustomerId);
                if (companyCustomer != null)
                {
                    // Fetch company defaults for this company
                    var companyDefaults = await _context.CompanyDefaults.FirstOrDefaultAsync(cd => cd.CompanyId == companyCustomer.CompanyId);
                    if (companyDefaults != null)
                    {
                        // Apply company defaults to rooms where paint type IDs are 0 or null
                        var hasChanges = false;
                        foreach (var room in entities)
                        {
                            if (!room.IncludeWalls || room.WallsPaintTypeId == 0 || room.WallsPaintTypeId == null)
                            {
                                room.WallsPaintTypeId = companyDefaults.PaintTypeWallsId;
                                hasChanges = true;
                            }

                            if (!room.IncludeCeilings || room.CeilingPaintTypeId == 0 || room.CeilingPaintTypeId == null)
                            {
                                room.CeilingPaintTypeId = companyDefaults.PaintTypeCeilingsId;
                                hasChanges = true;
                            }

                            if (!room.IncludeBaseboards || room.BaseboardsPaintTypeId == 0 || room.BaseboardsPaintTypeId == null)
                            {
                                room.BaseboardsPaintTypeId = companyDefaults.PaintTypeBaseboardsId;
                                hasChanges = true;
                            }

                            if (!room.IncludeDoors || room.DoorsPaintTypeId == 0 || room.DoorsPaintTypeId == null)
                            {
                                room.DoorsPaintTypeId = companyDefaults.PaintTypeTrimDoorsId;
                                hasChanges = true;
                            }

                            if (!room.IncludeWindows || room.WindowsPaintTypeId == 0 || room.WindowsPaintTypeId == null)
                            {
                                room.WindowsPaintTypeId = companyDefaults.PaintTypeWindowsId;
                                hasChanges = true;
                            }
                        }

                        // Save changes if any were made
                        if (hasChanges)
                            await _context.SaveChangesAsync();
                    }
                }
            }

            // Normalize each entity via the copy constructor to ensure defaults are applied
            var normalized = entities.Select(e => new Room(e)).ToList();

            return Ok(normalized);
        }

        // PUT: api/Rooms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, Room room)
        {
            if (id != room.Id)
            {
                return BadRequest();
            }

            // Need to update the room's Labor, Material and Overhead costs
            _context.Entry(room).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(room);
        }

        // POST: api/Rooms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Room>> PostRoom(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoom", new { id = room.Id }, room);
        }

        // DELETE: api/Rooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }
    }
}
