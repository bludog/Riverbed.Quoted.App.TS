namespace Riverbed.Pricing.Paint.Shared.Data
{
    // Minimal DTO for CompanyHTMLReportTemplate used across controllers and client services
    public class CompanyHTMLReportsTemplateMinimal
    {
        public int Id { get; set; }
        public string ReportName { get; set; } = string.Empty;
        public int ReportTypeId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
