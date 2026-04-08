using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;

namespace Riverbed.Pricing.Paint.Controllers.Project
{
    [ApiController]
    [Route("api/[controller]")]
    public class GlobalRoomSettingsController : ControllerBase
    {
        private readonly PricingDbContext _db;
        private readonly ILogger<GlobalRoomSettingsController> _logger;

        public GlobalRoomSettingsController(PricingDbContext db, ILogger<GlobalRoomSettingsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Updates inclusion flags (walls, ceilings, doors, windows, baseboards, crown molding)
        /// for every room in the specified project.
        /// Any flag left null will not be changed.
        /// </summary>
        /// <param name="projectId">The numeric ProjectData.Id</param>
        /// <param name="request">Inclusion flags to apply</param>
        [HttpPut("project/{projectId:int}/inclusions")]
        public async Task<IActionResult> UpdateRoomInclusions(int projectId, [FromBody] RoomInclusionsRequest request)
        {
            if (request is null)
                return BadRequest("Request body is required.");

            var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project is null)
                return NotFound($"Project {projectId} not found.");

            // Block changes for Accepted projects (StatusCodeId == 2)
            if (project.StatusCodeId == 2)
                return Conflict("Project is accepted. Global changes are not allowed.");

            var rooms = await _db.Rooms.Where(r => r.ProjectDataId == projectId).ToListAsync();
            if (rooms.Count == 0)
                return NotFound($"No rooms found for project id {projectId}.");

            foreach (var room in rooms)
            {
                if (request.IncludeWalls.HasValue) room.IncludeWalls = request.IncludeWalls.Value;
                if (request.IncludeCeilings.HasValue) room.IncludeCeilings = request.IncludeCeilings.Value;
                if (request.IncludeDoors.HasValue) room.IncludeDoors = request.IncludeDoors.Value;
                if (request.IncludeWindows.HasValue) room.IncludeWindows = request.IncludeWindows.Value;
                if (request.IncludeBaseboards.HasValue) room.IncludeBaseboards = request.IncludeBaseboards.Value;
                if (request.IncludeCrownMoldings.HasValue) room.IncludeCrownMoldings = request.IncludeCrownMoldings.Value;
            }

            await _db.SaveChangesAsync();

            _logger.LogDebug("Updated room inclusion flags for {Count} rooms in project {ProjectId}.", rooms.Count, projectId);
            return Ok(new { projectId, updatedRooms = rooms.Count });
        }
    }
}
