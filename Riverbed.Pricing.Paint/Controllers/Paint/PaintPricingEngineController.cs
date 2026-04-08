using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Shared;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;
using Riverbed.PricingEngines;

namespace Riverbed.Pricing.Paint.Controllers.Paint
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaintPricingEngineController : ControllerBase
    {
        private readonly PricingDbContext _pricingCtx;
        private readonly ILogger<PaintPricingEngineEx> _logger;

        public PaintPricingEngineController(PricingDbContext pricingCtx, ILogger<PaintPricingEngineEx> logger)
        {
            _pricingCtx = pricingCtx;
            _logger = logger;
        }
                

        [Route("Interior/{projectId}")]
        [HttpGet]
        public async Task<ActionResult<PricingResponseInterior>> GetInteriorPricingByProject(int projectId)
        {
            var proj = _pricingCtx.Projects.FirstOrDefault(p => p.Id == projectId);
            var rooms = await _pricingCtx.Rooms
                .Where(r => r.ProjectDataId == projectId && r.IsOptional == false)
                .Include(r => r.PaintableItems)
                .ToListAsync();
            if (rooms.Count == 0)
            {
                return Ok(new PricingResponseInterior());
            }

            var engine = new PaintPricingEngineEx(_logger);
            var cust = _pricingCtx.CompanyCustomers.FirstOrDefault(c => c.Id == proj.CompanyCustomerId);
            var pricingInteriorDefault = await _pricingCtx.PricingInteriorDefaults.FirstOrDefaultAsync(d => d.CompanyId == cust.CompanyId);
            var hourlyRate = _pricingCtx.CompanySettings.FirstOrDefault(c => c.CompanyId == cust.CompanyId).HourlyRate;

            // PaintType lookup delegate for engine
            async Task<(decimal pricePerGallon, int coveragePerGallon)> paintLookupAsync(int paintTypeId)
            {
                var pt = await _pricingCtx.PaintTypes.FirstOrDefaultAsync(p => p.Id == paintTypeId);
                if (pt == null)
                    return (39.27M, pricingInteriorDefault.PaintCoveragePerGallon);
                // Use CoverageOneCoatSqFt for 1 coat, CoverageTwoCoatsSqFt for 2 coats, fallback to CoverageOneCoatSqFt
                return ((decimal)pt.PricePerGallon, pt.CoverageOneCoatSqFt > 0 ? pt.CoverageOneCoatSqFt : pricingInteriorDefault.PaintCoveragePerGallon);
            }

            var responses = await engine.CalculateInteriorPaintPricing(rooms, pricingInteriorDefault, hourlyRate, paintLookupAsync);    
            var project = _pricingCtx.Projects.FirstOrDefault(p => p.Id == projectId);
            if (project != null)
            {
                project.BaseAmount = (decimal)responses.TotalCost;
                await _pricingCtx.SaveChangesAsync();
            }

            // Update the labor, Material and Overhead cost for the room
            foreach (var roomCost in responses.RoomPricingDetails)
            {
                var room = rooms.Where(r => r.Id == roomCost.RoomId).FirstOrDefault();
                await SaveRoomPricingValues(roomCost, room);
            }

            return Ok(responses);
        }
                

        [Route("Interior/Room/{roomId}")]
        [HttpGet]
        public async Task<ActionResult<PricingResponseInterior>> GetInteriorPricingByRoom(int roomId)
        {
            var rooms = await _pricingCtx.Rooms
                .Where(r => r.Id == roomId)
                .Include(r => r.PaintableItems)
                .ToListAsync();
            if (rooms.Count == 0)
            {
                return Ok(new PricingResponseInterior());
            }
            var proj = _pricingCtx.Projects.FirstOrDefault(p => p.Id == rooms.FirstOrDefault().ProjectDataId);

            var engine = new PaintPricingEngineEx(_logger);
            var cust = _pricingCtx.CompanyCustomers.FirstOrDefault(c => c.Id == proj.CompanyCustomerId);
            var pricingInteriorDefault = await _pricingCtx.PricingInteriorDefaults.FirstOrDefaultAsync(d => d.CompanyId == cust.CompanyId);
            var hourlyRate = _pricingCtx.CompanySettings.FirstOrDefault(c => c.CompanyId == cust.CompanyId).HourlyRate;

            async Task<(decimal pricePerGallon, int coveragePerGallon)> paintLookupAsync(int paintTypeId)
            {
                var pt = await _pricingCtx.PaintTypes.FirstOrDefaultAsync(p => p.Id == paintTypeId);
                if (pt == null)
                    return (39.27M, pricingInteriorDefault.PaintCoveragePerGallon);
                return ((decimal)pt.PricePerGallon, pt.CoverageOneCoatSqFt > 0 ? pt.CoverageOneCoatSqFt : pricingInteriorDefault.PaintCoveragePerGallon);
            }

            var responses = await engine.CalculateInteriorPaintPricing(rooms, pricingInteriorDefault, hourlyRate, paintLookupAsync);

            var roomCost = responses.RoomPricingDetails.FirstOrDefault();
            await SaveRoomPricingValues(roomCost, rooms.FirstOrDefault());

            await UpdateProjectTotalCost(proj);

            return Ok(responses.RoomPricingDetails.FirstOrDefault());
        }

        private async Task UpdateProjectTotalCost(ProjectData? proj)
        {
            // Update the Project TotalCost using the sum of all room costs and do not include optional rooms            
            if (proj != null)
            {
                var totalCost = await _pricingCtx.Rooms
                    .Where(r => r.ProjectDataId == proj.Id && r.IsOptional == false)
                    .SumAsync(r => r.LaborCost + r.MaterialCost);
                proj.BaseAmount = (decimal)totalCost;
                await _pricingCtx.SaveChangesAsync();
            }
        }

        // Save the room pricing values
        private async Task SaveRoomPricingValues(RoomPricingDetail roomCost, Room? room)
        {
            var interiorDefaults = _pricingCtx.CompanySettings.FirstOrDefault();
            if (room != null)
            {
                room.LaborCost = (float)(roomCost.WallsCost + roomCost.CeilingCost + roomCost.DoorsCost + roomCost.BaseboardsCost + roomCost.CrownMoldingsCost + roomCost.AdditionalCost);
                room.MaterialCost = (float)(roomCost.PaintCost);
                room.OverheadCost = (float)(roomCost.TotalCost - (room.LaborCost + room.MaterialCost));
                room.Profit = (float)(roomCost.TotalCost - room.LaborCost - room.MaterialCost);

                await _pricingCtx.SaveChangesAsync();
            }
        }

        [Route("GetExteriorPaintPricing")]
        [HttpPost]
        public ActionResult<PricingResponseExterior> GetPricing([FromBody] PricingRequestExterior request)
        {
            var engine = new PaintPricingEngineEx(_logger);
            var response = engine.CalculateExteriorPaintPricing(request);
            return Ok(response);
        }       
    }
}
