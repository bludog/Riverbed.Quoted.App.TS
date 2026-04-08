using Azure.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Riverbed.Pricing.Paint.Shared;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;
using Riverbed.Pricing.Paint.Shared.Services;
using static Riverbed.PricingEngines.PaintPricingEngineEx;

namespace Riverbed.PricingEngines
{
    public class PaintPricingEngineEx
    {
        private readonly ILogger<PaintPricingEngineEx> _logger;
        private float GetPaintQualityMultiplier(int quality)
        {
            var paintQuality = (PaintQuality)quality;
            return paintQuality switch
            {
                PaintQuality.Standard => 1.0f,
                PaintQuality.High => 1.3f,
                PaintQuality.Premium => 1.5f,
                _ => throw new ArgumentException("Invalid paint quality", nameof(quality))
            }            ;
        }

        public enum PaintQuality
        {
            Standard = 1,
            High,
            Premium
        }

        public PaintPricingEngineEx(ILogger<PaintPricingEngineEx> logger)
        {
            _logger = logger;
        }
               
        public async Task<PricingResponseInterior> CalculateInteriorPaintPricing(
            List<Room> rooms,
            PricingInteriorDefault interiorDefaults,
            float hourlyRate,
            Func<int, Task<(decimal pricePerGallon, int coveragePerGallon)>> paintLookupAsync)
        {
            if (rooms == null || !rooms.Any())
                throw new ArgumentException("Rooms list cannot be null or empty.", nameof(rooms));

            if (interiorDefaults == null)
                throw new ArgumentNullException(nameof(interiorDefaults));

            // fallback function when paintLookupAsync is null
            if (paintLookupAsync == null)
            {
                paintLookupAsync = id => Task.FromResult<(decimal, int)>((39.27M, interiorDefaults.PaintCoveragePerGallon));
            }

            var response = new PricingResponseInterior { RoomPricingDetails = new List<RoomPricingDetail>() };

            // cache paint lookups to avoid repeated calls
            var paintCache = new Dictionary<int, (decimal price, int coverage)>();

            async Task<(decimal price, int coverage)> GetPaintInfo(int paintTypeId)
            {
                if (paintTypeId <= 0)
                    return (39.27M, interiorDefaults.PaintCoveragePerGallon);

                if (paintCache.TryGetValue(paintTypeId, out var info))
                    return info;

                var (price, coverage) = await paintLookupAsync(paintTypeId);
                paintCache[paintTypeId] = (price, coverage);
                return (price, coverage);
            }

            var totalProjectCost = 0.0M;
            foreach (var room in rooms)
            {
                if (room.Length <= 0 || room.Width <= 0 || room.Height <= 0)
                    throw new ArgumentException($"Invalid dimensions for room: {room.Name}");

                // geometry
                var wallsArea = 2 * (room.Length + room.Width) * room.Height; // sq ft
                var ceilingArea = room.IncludeCeilings ? room.Length * room.Width : 0; // sq ft
                var baseboardLength = room.IncludeBaseboards ? 2 * (room.Length + room.Width) : 0; // linear ft
                var crownMoldingLength = room.IncludeCrownMoldings ? 2 * (room.Length + room.Width) : 0; // linear ft

                // additional prep/labor
                float additionalPrepTime = room.AdditionalPrepTime ?? 0f;
                decimal additionalCost = (decimal)(additionalPrepTime * hourlyRate);

                if (room.PaintableItems != null && room.PaintableItems.Any())
                {
                    foreach (var item in room.PaintableItems)
                    {
                        additionalCost += (decimal)(item.Count * item.Price);
                    }
                }

                // Per-surface paint gallons and costs
                double wallsGallons = 0, ceilingGallons = 0, baseboardsGallons = 0, doorsGallons = 0, crownGallons = 0, windowsGallons = 0;
                decimal wallsPaintCost = 0, ceilingPaintCost = 0, baseboardsPaintCost = 0, doorsPaintCost = 0, crownPaintCost = 0, windowsPaintCost = 0;

                // WALLS
                if (room.IncludeWalls)
                {
                    var (pricePerGal, coverage) = await GetPaintInfo(room.WallsPaintTypeId ?? 0);
                    var coverageD = coverage > 0 ? coverage : interiorDefaults.PaintCoveragePerGallon;
                    // gallons = ceil( area / coverage ) * coats
                    wallsGallons = Math.Ceiling((double)wallsArea / coverageD) * Math.Max(1, (int)(room.WallsCoats ?? 1));
                    wallsPaintCost = (decimal)wallsGallons * pricePerGal * (decimal)GetPaintQualityMultiplier(room.PaintQualityId);
                }

                // CEILING
                if (room.IncludeCeilings)
                {
                    var (pricePerGal, coverage) = await GetPaintInfo(room.CeilingPaintTypeId ?? 0);
                    var coverageD = coverage > 0 ? coverage : interiorDefaults.PaintCoveragePerGallon;
                    ceilingGallons = Math.Ceiling((double)ceilingArea / coverageD) * Math.Max(1, (int)(room.CeilingCoats ?? 1));
                    ceilingPaintCost = (decimal)ceilingGallons * pricePerGal * (decimal)GetPaintQualityMultiplier(room.PaintQualityId);
                }

                // BASEBOARDS (use linear feet -> convert coverage assumption: gallons per linear foot)
                if (room.IncludeBaseboards)
                {
                    var (pricePerGal, coverage) = await GetPaintInfo(room.BaseboardsPaintTypeId ?? 0);
                    // assume coverage for linear feet: interiorDefaults.PaintCoveragePerGallon (sqft) -> we use 100 linear ft per gal as existing heuristic, or if coverage supplied treat as linear coverage
                    double gallonsPer100Lf = 1.0;
                    baseboardsGallons = Math.Ceiling(baseboardLength / 100.0) * Math.Max(1, (int)(room.BaseboardsCoats ?? 1));
                    baseboardsPaintCost = (decimal)baseboardsGallons * pricePerGal * (decimal)GetPaintQualityMultiplier(room.PaintQualityId);
                }

                // DOORS
                if (room.IncludeDoors && room.NumberOfDoors > 0)
                {
                    var (pricePerGal, coverage) = await GetPaintInfo(room.DoorsPaintTypeId ?? 0);
                    // keep previous heuristic (0.25 gal per door) but multiply by coats
                    doorsGallons = room.NumberOfDoors * 0.25 * Math.Max(1, (int)(room.DoorsCoats ?? 1));
                    doorsPaintCost = (decimal)doorsGallons * pricePerGal * (decimal)GetPaintQualityMultiplier(room.PaintQualityId);
                }

                // CROWN MOLDINGS
                if (room.IncludeCrownMoldings)
                {
                    var (pricePerGal, coverage) = await GetPaintInfo(room.BaseboardsPaintTypeId ?? 0); // or a dedicated crown paint type
                    crownGallons = Math.Ceiling(crownMoldingLength / 100.0) * Math.Max(1, (int)(room.BaseboardsCoats ?? 1));
                    crownPaintCost = (decimal)crownGallons * pricePerGal * (decimal)GetPaintQualityMultiplier(room.PaintQualityId);
                }

                // WINDOWS (if you paint sash or trim)
                if (room.IncludeWindows && room.NumberOfWindows > 0)
                {
                    var (pricePerGal, coverage) = await GetPaintInfo(room.WindowsPaintTypeId ?? 0);
                    windowsGallons = room.NumberOfWindows * 0.05 * Math.Max(1, (int)(room.WindowsCoats ?? 1)); // example: 0.05 gal per window
                    windowsPaintCost = (decimal)windowsGallons * pricePerGal * (decimal)GetPaintQualityMultiplier(room.PaintQualityId);
                }

                // Existing labor component calculations (kept but consider refining)
                var wallsCost = room.IncludeWalls ? (decimal)((wallsArea / interiorDefaults.WallRatePerSquareFoot) * hourlyRate) : 0.0M;
                var ceilingCost = room.IncludeCeilings ? (decimal)((ceilingArea / interiorDefaults.CeilingRatePerSquareFoot) * hourlyRate) : 0.0M;
                var doorsCost = room.IncludeDoors ? (decimal)(room.NumberOfDoors * (hourlyRate * interiorDefaults.DoorRateEach)) : 0.0M;
                var baseboardsCost = room.IncludeBaseboards ? (decimal)((baseboardLength / interiorDefaults.BaseboardRatePerLinearFoot) * hourlyRate) : 0.0M;
                var crownMoldingsCost = room.IncludeCrownMoldings ? (decimal)((crownMoldingLength / interiorDefaults.CrownMoldingRatePerLinearFoot) * hourlyRate) : 0.0M;

                // Sum paint material costs from all surfaces
                var totalPaintCost = wallsPaintCost + ceilingPaintCost + baseboardsPaintCost + doorsPaintCost + crownPaintCost + windowsPaintCost;

                // Optional: increase labor cost proportionally to coats (simple example)
                decimal coatsLaborMultiplier = 1.0M + (decimal)(Math.Max(0, (int)(room.WallsCoats ?? 1) - 1) * 0.1); // 10% extra per additional coat
                var laborAdjusted = (wallsCost + ceilingCost + baseboardsCost + crownMoldingsCost + doorsCost) * coatsLaborMultiplier;


                var roomPricingDetail = new RoomPricingDetail
                {
                    RoomName = room.Name,
                    TotalCost = (double)(laborAdjusted + additionalCost + totalPaintCost),
                    WallsCost = (double)wallsCost,
                    CeilingCost = (double)ceilingCost,
                    DoorsCost = (double)doorsCost,
                    BaseboardsCost = (double)baseboardsCost,
                    CrownMoldingsCost = (double)crownMoldingsCost,
                    AdditionalCost = (double)additionalCost,
                    RoomId = room.Id,
                    IsOptional = room.IsOptional,
                    PaintCost = (double)totalPaintCost,
                    PaintInteriorRequirements = new PaintInteriorRequirement
                    {
                        WallsPaint = wallsGallons,
                        CeilingPaint = ceilingGallons,
                        DoorsPaint = doorsGallons,
                        BaseboardsPaint = baseboardsGallons,
                        CrownMoldingsPaint = crownGallons
                    }
                };
              
                response.RoomPricingDetails.Add(roomPricingDetail);
            }

            return response;
        }

        public async Task<PricingResponseExterior> CalculateExteriorPaintPricing(PricingRequestExterior request)
        {
            var exteriorDefaults = new PricingExteriorDefault();
            double doorsCost = request.ExteriorDoorCount * exteriorDefaults.ExteriorDoorRate;
            double singleGarageDoorsCost = request.SingleGarageDoors * exteriorDefaults.SingleGarageDoorRate;
            double doubleGarageDoorsCost = request.DoubleGarageDoors * exteriorDefaults.DoubleGarageDoorRate;

            // Assuming the length and width are the total perimeter for boxing and siding calculation
            double perimeter = 2 * (request.Length + request.Width);

            double boxingArea = request.IncludeBoxing ? perimeter * 2 : 0;
            double sidingArea = request.IncludeSiding ? perimeter * request.HeightToRoofBase : 0;

            var heightMultiplier = request.HeightToRoofBase / 10;
            double boxingCost = request.IncludeBoxing ? perimeter * (heightMultiplier * exteriorDefaults.BoxingRatePerLinearFoot) : 0;
            double sidingCost = request.IncludeSiding ? perimeter * request.HeightToRoofBase * exteriorDefaults.SidingRatePerSquareFoot : 0;
            double chimneyCost = request.IncludeChimney ? exteriorDefaults.ChimneyRateFlat : 0;

            // Calculate paint requirements
            var gallonMultiplier = exteriorDefaults.PaintCoats * 2; // Assuming 2 coats
            double sidingPaintGallons = Math.Ceiling(sidingArea / exteriorDefaults.PaintCoveragePerGallon);
            double boxingPaintGallons = Math.Ceiling(boxingArea / exteriorDefaults.PaintCoveragePerGallon);
            double doorsPaintGallons = request.ExteriorDoorCount * 0.5; // Assuming 0.5 gallons per door
            double garageDoorsPaintGallons = request.SingleGarageDoors * 0.75 + request.DoubleGarageDoors * 1.5; // Assuming 0.75 gallons for single, 1.5 gallons for double

            var paintGallonCost = 32.00 * GetPaintQualityMultiplier(request.PaintQualityId);  // Example cost per gallon 
            double totalMaterialCost = (sidingPaintGallons + boxingPaintGallons + doorsPaintGallons + garageDoorsPaintGallons) * paintGallonCost;
           
            return new PricingResponseExterior
            {
                TotalCost = doorsCost + singleGarageDoorsCost + doubleGarageDoorsCost + boxingCost + sidingCost + chimneyCost,
                DoorsCost = doorsCost,
                GarageDoorsCost = singleGarageDoorsCost + doubleGarageDoorsCost,
                BoxingCost = boxingCost,
                SidingCost = sidingCost,
                ChimneyCost = chimneyCost,
                TotalMaterialCost = totalMaterialCost,
                PaintExteriorRequirements = new PaintExteriorRequirement
                {
                    SidingPaintGallons = sidingPaintGallons,
                    BoxingPaintGallons = boxingPaintGallons,
                    DoorsPaintGallons = doorsPaintGallons,
                    GarageDoorsPaintGallons = garageDoorsPaintGallons
                }
            };
        }
    }
}
