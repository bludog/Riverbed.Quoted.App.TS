namespace Riverbed.Pricing.Paint.Shared
{
    public class PricingInteriorDefault
    {
        public int Id { get; set; }
        public float WallRatePerSquareFoot {get; set;}
        public float CeilingRatePerSquareFoot { get; set; }
        public float DoorRateEach { get; set; }
        public float BaseboardRatePerLinearFoot { get; set; }
        public float CrownMoldingRatePerLinearFoot { get; set; }
        public float WindowRateEach { get; set; }
        public int PaintCoveragePerGallon { get; set; }
        public int PaintCoats { get; set; }

        //public float WallRatePerSquareFoot = 0.65f;
        //public float CeilingRatePerSquareFoot = 1.25f;
        //public float DoorRateEach = 35.00f;
        //public float BaseboardRatePerLinearFoot = 2.25f;
        //public float CrownMoldingRatePerLinearFoot = 1.50f;
        //public int PaintCoveragePerGallon = 300; // Example value
        //public int PaintCoats = 2;
        public Guid CompanyId { get; set; } = Guid.NewGuid();
    }
}
