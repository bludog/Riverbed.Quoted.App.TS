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
    public class PaintQualitiesController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public PaintQualitiesController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/PaintQualities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaintQuality>>> GetPaintQualities()
        {
            return await _context.PaintQualities.ToListAsync();
        }

        // GET: api/PaintQualities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaintQuality>> GetPaintQuality(int id)
        {
            var paintQuality = await _context.PaintQualities.FindAsync(id);

            if (paintQuality == null)
            {
                return NotFound();
            }

            return paintQuality;
        }

        // PUT: api/PaintQualities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaintQuality(int id, PaintQuality paintQuality)
        {
            if (id != paintQuality.Id)
            {
                return BadRequest();
            }

            _context.Entry(paintQuality).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaintQualityExists(id))
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

        /// <summary>
        /// Update paint quality id for all rooms in a project.
        /// Accepts the new paintQualityId as a route parameter to simplify client calls.
        /// </summary>
        [HttpPut("project/{projectId:int}/global/paintquality/{paintQualityId:int}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GlobalUpdateRoomPaintQuality(int projectId, int paintQualityId)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project is null)
                return NotFound($"Project {projectId} not found.");

            // Block changes for Accepted projects (StatusCodeId == 2)
            if (project.StatusCodeId == 2)
                return Conflict("Project is accepted. Global changes are not allowed.");

            var rooms = await _context.Rooms.Where(r => r.ProjectDataId == projectId).ToListAsync();
            if (rooms.Count == 0)
                return NotFound($"No rooms found for project id {projectId}.");

            foreach (var room in rooms)
            {
                room.PaintQualityId = paintQualityId;
            }

            await _context.SaveChangesAsync();

            //_logger.LogDebug("Updated PaintQualityId to {PaintQualityId} for {Count} rooms in project {ProjectId}.", paintQualityId, rooms.Count, projectId);
            // Return the number of rooms updated as a simple integer in the response body
            return Ok(rooms.Count);
        }

        // POST: api/PaintQualities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PaintQuality>> PostPaintQuality(PaintQuality paintQuality)
        {
            _context.PaintQualities.Add(paintQuality);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPaintQuality", new { id = paintQuality.Id }, paintQuality);
        }

        // DELETE: api/PaintQualities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaintQuality(int id)
        {
            var paintQuality = await _context.PaintQualities.FindAsync(id);
            if (paintQuality == null)
            {
                return NotFound();
            }

            _context.PaintQualities.Remove(paintQuality);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaintQualityExists(int id)
        {
            return _context.PaintQualities.Any(e => e.Id == id);
        }
    }
}
