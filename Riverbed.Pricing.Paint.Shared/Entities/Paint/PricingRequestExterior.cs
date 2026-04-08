namespace Riverbed.Pricing.Paint.Shared
{
    public class PricingRequestExterior
    {
        public int Id { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double HeightToRoofBase { get; set; }
        public int ExteriorDoorCount { get; set; }
        public int SingleGarageDoors { get; set; }
        public int DoubleGarageDoors { get; set; }
        public bool IncludeBoxing { get; set; }
        public bool IncludeSiding { get; set; }
        public bool IncludeChimney { get; set; }
        public int PaintQualityId { get; set; }
        public AllPaintQuality PaintQuality { get; set; }
        public Guid CompanyId { get; set; }
    }
}
