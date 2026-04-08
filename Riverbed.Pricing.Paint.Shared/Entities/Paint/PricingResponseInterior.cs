namespace Riverbed.Pricing.Paint.Shared
{
    public class PricingResponseInterior
    {
        public int Id { get; set; }
        public List<RoomPricingDetail> RoomPricingDetails { get; set; } = new List<RoomPricingDetail>();
        public double TotalCost => RoomPricingDetails.Sum(r => (r.IsOptional ? 0: r.TotalCost));
    }

    public class RoomPricingDetail
    {
        public int Id { get; set; }
        public string RoomName { get; set; }
        public int RoomId { get; set; }
        public double TotalCost { get; set; }
        public double WallsCost { get; set; }
        public double CeilingCost { get; set; }
        public double DoorsCost { get; set; }
        public double BaseboardsCost { get; set; }
        public double CrownMoldingsCost { get; set; }
        public double AdditionalCost { get; set; }
        public double PaintCost { get; set; }
        public int PricingRequestInteriorId { get; set; }
        public int PaintRequirementsId { get; set; }
        public bool IsOptional { get; set; }
        public PaintInteriorRequirement PaintInteriorRequirements { get; set; }
    }   
}
