using System;

namespace Riverbed.Pricing.Paint.Shared.Data
{
    public class ProjectBoardItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Project { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal HalfDown { get; set; }
        public decimal OrderPaint { get; set; }
        public decimal Paint { get; set; }
        public decimal TotalJob { get; set; }
        public decimal Labor { get; set; }
        public decimal Materials { get; set; }
        public decimal LaborPaid { get; set; }
        public decimal CustomerPaid { get; set; }
        public int InvoiceStatusId { get; set; }
        public int WorkingStatusId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
    }
}
