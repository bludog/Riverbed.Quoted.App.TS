namespace Riverbed.Pricing.Paint.Shared
{
    public class PricingExteriorDefault
    {
        public int Id { get; set; }        
        public double ExteriorDoorRate { get; set; } = 100.00;
        public double SingleGarageDoorRate { get; set; } = 200.00;
        public double DoubleGarageDoorRate { get; set; } = 300.00;
        public double BoxingRatePerLinearFoot { get; set; } = 6.00;
        public double SidingRatePerSquareFoot { get; set; } = 2.50;
        public double ChimneyRateFlat { get; set; } = 500.00;
        public int PaintCoveragePerGallon { get; set; } = 350;
        public int PaintCoats { get; set; } = 2;
        public Guid CompanyId { get; set; } = new Guid(); 
    }
}
