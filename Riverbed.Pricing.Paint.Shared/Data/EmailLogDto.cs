using System;

namespace Riverbed.Pricing.Paint.Shared.Data
{
    // Minimal DTO for displaying email logs in the UI
    public class EmailLogDto
    {
        public int Id { get; set; }
        public DateTime TimeSent { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string ToEmail { get; set; } = string.Empty;
        public string? CcEmail { get; set; }
        public string? BccEmail { get; set; }
        public string? EmailTypeName { get; set; }
    }
}
