namespace Riverbed.Pricing.Paint.Shared
{
    public class PricingResponseExterior
    {
        public int Id { get; set; }
        public double TotalCost { get; set; }
        public double DoorsCost { get; set; }
        public double GarageDoorsCost { get; set; }
        public double BoxingCost { get; set; }
        public double SidingCost { get; set; }
        public double ChimneyCost { get; set; }
        public double TotalMaterialCost { get; set; }
        public int PricingRequestExteriorId { get; set; }
        public int PaintExteriorRequirementsId { get; set; }
        public PaintExteriorRequirement PaintExteriorRequirements { get; set; }
    }
    
}
