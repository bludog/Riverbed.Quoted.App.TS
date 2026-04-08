using System;

namespace Riverbed.Pricing.Paint.Shared.Data
{
    /// <summary>
    /// Minimal projection for projects used in lists and lookups
    /// </summary>
    public class ProjectDataMinimal
    {
        public string ProjectName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public Guid ProjectGuid { get; set; }
        public int StatusCodeId { get; set; }
    }
}
