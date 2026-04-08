using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Riverbed.Pricing.Paint.Shared.Entities.Reporting
{
    /// <summary>
    /// Represents an HTML report template associated with a company
    /// </summary>
    public class CompanyHTMLReport
    {
        /// <summary>
        /// Primary key for the company HTML report
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The unique identifier of the company this report is associated with
        /// </summary>
        [Required]
        public Guid CompanyGuid { get; set; }

        [Required]
        public Guid ProjectGuid { get; set; }

        /// <summary>
        /// The name of the company this report is associated with
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string CompanyName { get; set; }

        /// <summary>
        /// The type of report (e.g., Quote, Invoice, etc.)
        /// </summary>
        [Required]
        public int ReportTypeId { get; set; }

        /// <summary>
        /// The HTML content of the report
        /// </summary>
        [Required]
        public string ReportHTMLText { get; set; }

        /// <summary>
        /// Indicates whether this is a global template that can be used by all companies
        /// </summary>
        public bool IsGlobalTemplate { get; set; }

        public bool IsActive { get; set; }

        public int DisplayOrder { get; set; }

        /// <summary>
        /// The date and time when the report was last updated
        /// </summary>
        [Required]
        public DateTime LastUpdatedDateTime { get; set; }
    }
}