using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;

namespace Riverbed.Pricing.Paint.Controllers.Surfaces
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomSurfacePaintLayersController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public RoomSurfacePaintLayersController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/RoomSurfacePaintLayers?roomSurfaceId=3
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RoomSurfacePaintLayer>>> GetRoomSurfacePaintLayers(
            [FromQuery] int? roomSurfaceId, CancellationToken cancellationToken)
        {
            try
            {
                IQueryable<RoomSurfacePaintLayer> query = _context.RoomSurfacePaintLayers;

                if (roomSurfaceId.HasValue)
                {
                    query = query.Where(l => l.RoomSurfaceId == roomSurfaceId.Value);
                }

                return await query.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/RoomSurfacePaintLayers/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RoomSurfacePaintLayer>> GetRoomSurfacePaintLayer(int id, CancellationToken cancellationToken)
        {
            try
            {
                var roomSurfacePaintLayer = await _context.RoomSurfacePaintLayers.FindAsync([id], cancellationToken);

                if (roomSurfacePaintLayer == null)
                {
                    return NotFound();
                }

                return roomSurfacePaintLayer;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT: api/RoomSurfacePaintLayers/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutRoomSurfacePaintLayer(int id, RoomSurfacePaintLayer roomSurfacePaintLayer, CancellationToken cancellationToken)
        {
            if (id != roomSurfacePaintLayer.RoomSurfacePaintLayerId)
            {
                return BadRequest();
            }

            try
            {
                if (roomSurfacePaintLayer.PaintTypeId.HasValue &&
                    !await _context.CompanyPaintTypes.AnyAsync(pt => pt.Id == roomSurfacePaintLayer.PaintTypeId.Value, cancellationToken))
                {
                    return BadRequest("Invalid PaintTypeId.");
                }

                if (!await _context.RoomSurfaces.AnyAsync(rs => rs.RoomSurfaceId == roomSurfacePaintLayer.RoomSurfaceId, cancellationToken))
                {
                    return BadRequest("Invalid RoomSurfaceId.");
                }

                var existing = await _context.RoomSurfacePaintLayers.FindAsync([id], cancellationToken);
                if (existing == null)
                {
                    return NotFound();
                }

                existing.PaintTypeId = roomSurfacePaintLayer.PaintTypeId;
                existing.LayerType = roomSurfacePaintLayer.LayerType;
                existing.Coats = roomSurfacePaintLayer.Coats;
                existing.WasteFactor = roomSurfacePaintLayer.WasteFactor;
                existing.GallonsCalculated = roomSurfacePaintLayer.GallonsCalculated;
                existing.CostCalculated = roomSurfacePaintLayer.CostCalculated;
                existing.SortOrder = roomSurfacePaintLayer.SortOrder;
                existing.Notes = roomSurfacePaintLayer.Notes;
                existing.UpdatedUtc = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await RoomSurfacePaintLayerExistsAsync(id, cancellationToken))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/RoomSurfacePaintLayers
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RoomSurfacePaintLayer>> PostRoomSurfacePaintLayer(RoomSurfacePaintLayer roomSurfacePaintLayer, CancellationToken cancellationToken)
        {
            try
            {
                if (roomSurfacePaintLayer.PaintTypeId.HasValue &&
                    !await _context.CompanyPaintTypes.AnyAsync(pt => pt.Id == roomSurfacePaintLayer.PaintTypeId.Value, cancellationToken))
                {
                    return BadRequest("Invalid PaintTypeId.");
                }

                if (!await _context.RoomSurfaces.AnyAsync(rs => rs.RoomSurfaceId == roomSurfacePaintLayer.RoomSurfaceId, cancellationToken))
                {
                    return BadRequest("Invalid RoomSurfaceId.");
                }

                _context.RoomSurfacePaintLayers.Add(roomSurfacePaintLayer);
                await _context.SaveChangesAsync(cancellationToken);

                return CreatedAtAction("GetRoomSurfacePaintLayer",
                    new { id = roomSurfacePaintLayer.RoomSurfacePaintLayerId }, roomSurfacePaintLayer);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: api/RoomSurfacePaintLayers/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRoomSurfacePaintLayer(int id, CancellationToken cancellationToken)
        {
            try
            {
                var roomSurfacePaintLayer = await _context.RoomSurfacePaintLayers.FindAsync([id], cancellationToken);
                if (roomSurfacePaintLayer == null)
                {
                    return NotFound();
                }

                _context.RoomSurfacePaintLayers.Remove(roomSurfacePaintLayer);
                await _context.SaveChangesAsync(cancellationToken);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private async Task<bool> RoomSurfacePaintLayerExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.RoomSurfacePaintLayers.AnyAsync(e => e.RoomSurfacePaintLayerId == id, cancellationToken);
        }
    }
}
