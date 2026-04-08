namespace Riverbed.Pricing.Paint.Shared.Contracts
{
    // API contract for merging HTML with template tokens
    public class MergeHtmlRequest
    {
        public string CompanyGuid { get; set; } = string.Empty;
        public string ProjectGuid { get; set; } = string.Empty;
        public string Html { get; set; } = string.Empty;
    }
}