using Azure.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Riverbed.Pricing.Paint.Shared;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;
using Riverbed.Pricing.Paint.Shared.Services;

namespace Riverbed.PricingEngines
{
    public class PaintPricingEngine
    {
        private readonly ILogger<PaintPricingEngine> _logger;

        private readonly Dictionary<AllPaintQuality, double> paintQualityMultipliers = new Dictionary<AllPaintQuality, double>
    {
        { AllPaintQuality.Standard, 1.0 },
        { AllPaintQuality.Premium, 1.3 },
        { AllPaintQuality.Luxury, 1.5 },
    };

        public PaintPricingEngine(ILogger<PaintPricingEngine> logger)
        {
            _logger = logger;
        }

        //public async Task<PricingResponseInterior> CalculateInteriorPaintPricing(PricingRequestInterior request, PricingInteriorDefault interiorDefaults)
        //{
        //    var response = new PricingResponseInterior { RoomPricingDetails = new List<RoomPricingDetail>() };
                  
        //    foreach (var room in request.Rooms)
        //    {
        //        double wallsArea = 2 * (room.Length + room.Width) * room.Height;
        //        double ceilingArea = room.IncludeCeilings ? room.Length * room.Width : 0;
        //        double baseboardLength = room.IncludeBaseboards ? 2 * (room.Length + room.Width) : 0;
        //        double crownMoldingLength = room.IncludeCrownMoldings ? 2 * (room.Length + room.Width) : 0;                

        //        var paintQuality = paintQualityMultipliers[(AllPaintQuality)room.PaintQualityId];
        //        double wallsCost = wallsArea * interiorDefaults.WallRatePerSquareFoot * paintQuality;
        //        double ceilingCost = ceilingArea * interiorDefaults.CeilingRatePerSquareFoot * paintQuality;
        //        double doorsCost = room.NumberOfDoors * interiorDefaults.DoorRateEach * paintQuality;
        //        double baseboardsCost = baseboardLength * interiorDefaults.BaseboardRatePerLinearFoot * paintQuality;
        //        double crownMoldingsCost = crownMoldingLength * interiorDefaults.CrownMoldingRatePerLinearFoot * paintQuality;

        //        var roomPricingDetail = new RoomPricingDetail
        //        {
        //            RoomName = room.Name,
        //            TotalCost = wallsCost + ceilingCost + doorsCost + baseboardsCost + crownMoldingsCost,
        //            WallsCost = wallsCost,
        //            CeilingCost = ceilingCost,
        //            DoorsCost = doorsCost,
        //            BaseboardsCost = baseboardsCost,
        //            CrownMoldingsCost = crownMoldingsCost,
        //            RoomId = room.Id,
        //            PaintInteriorRequirements = new PaintInteriorRequirement
        //            {
        //                WallsPaint = Math.Ceiling(wallsArea / interiorDefaults.PaintCoveragePerGallon),
        //                CeilingPaint = Math.Ceiling(ceilingArea / interiorDefaults.PaintCoveragePerGallon),
        //                DoorsPaint = room.NumberOfDoors * 0.25, // Assuming 0.25 gallons per door as an example
        //                BaseboardsPaint = Math.Ceiling(baseboardLength / 100), // Assuming 1 gallon per 100 linear feet
        //                CrownMoldingsPaint = Math.Ceiling(crownMoldingLength / 100)
        //            }
        //        };

        //        response.RoomPricingDetails.Add(roomPricingDetail);
        //    }

        //    return response;
        //}

        public async Task<PricingResponseInterior> CalculateInteriorPaintPricing(List<Room> rooms, PricingInteriorDefault interiorDefaults, float hourlyRate)
        {
            var response = new PricingResponseInterior { RoomPricingDetails = new List<RoomPricingDetail>() };
              
            foreach (var room in rooms)
            {
                double wallsArea = 2 * (room.Length + room.Width) * room.Height;
                double ceilingArea = room.IncludeCeilings ? room.Length * room.Width : 0;
                double baseboardLength = room.IncludeBaseboards ? 2 * (room.Length + room.Width) : 0;
                double crownMoldingLength = room.IncludeCrownMoldings ? 2 * (room.Length + room.Width) : 0;

                var paintQuality = paintQualityMultipliers[(AllPaintQuality)room.PaintQualityId];
                double wallsCost = wallsArea * interiorDefaults.WallRatePerSquareFoot * paintQuality;
                double ceilingCost = ceilingArea * interiorDefaults.CeilingRatePerSquareFoot * paintQuality;
                double doorsCost = room.NumberOfDoors * interiorDefaults.DoorRateEach * paintQuality;
                double baseboardsCost = baseboardLength * interiorDefaults.BaseboardRatePerLinearFoot * paintQuality;
                double crownMoldingsCost = crownMoldingLength * interiorDefaults.CrownMoldingRatePerLinearFoot * paintQuality;

                var roomPricingDetail = new RoomPricingDetail
                {
                    RoomName = room.Name,
                    TotalCost = wallsCost + ceilingCost + doorsCost + baseboardsCost + crownMoldingsCost,
                    WallsCost = wallsCost,
                    CeilingCost = ceilingCost,
                    DoorsCost = doorsCost,
                    BaseboardsCost = baseboardsCost,
                    CrownMoldingsCost = crownMoldingsCost,
                    RoomId = room.Id,
                    PaintInteriorRequirements = new PaintInteriorRequirement
                    {
                        WallsPaint = Math.Ceiling(wallsArea / interiorDefaults.PaintCoveragePerGallon),
                        CeilingPaint = Math.Ceiling(ceilingArea / interiorDefaults.PaintCoveragePerGallon),
                        DoorsPaint = room.NumberOfDoors * 0.25, // Assuming 0.25 gallons per door as an example
                        BaseboardsPaint = Math.Ceiling(baseboardLength / 100), // Assuming 1 gallon per 100 linear feet
                        CrownMoldingsPaint = Math.Ceiling(crownMoldingLength / 100)
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

            var paintGallonCost = 32.00 * paintQualityMultipliers[request.PaintQuality]; // Example cost per gallon 
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
