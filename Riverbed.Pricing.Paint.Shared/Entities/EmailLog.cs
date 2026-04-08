using System;
using System.ComponentModel.DataAnnotations;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class EmailLog
    {
        [Key]
        public int Id { get; set; }

        public DateTime TimeSent { get; set; } = DateTime.UtcNow;

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string EmailBody { get; set; } = string.Empty;

        public int? CompanyReportTypeId { get; set; }

        public Guid? ProjectGuid { get; set; }

        public Guid? CompanyGuid { get; set; }

        [Required, MaxLength(512)]
        public string ToEmail { get; set; } = string.Empty;

        [MaxLength(512)]
        public string? CcEmail { get; set; }

        [MaxLength(512)]
        public string? BccEmail { get; set; }

        [Required, MaxLength(512)]
        public string FromEmail { get; set; } = string.Empty;
    }
}
