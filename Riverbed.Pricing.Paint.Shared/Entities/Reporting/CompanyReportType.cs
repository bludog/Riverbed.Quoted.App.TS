using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Riverbed.Pricing.Paint.Shared.Entities.Reporting
{
    /// <summary>
    /// Represents a type of report that can be associated with company HTML reports
    /// </summary>
    public class CompanyReportType
    {
        /// <summary>
        /// Primary key for the report type
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of the report type (e.g., Quote, Invoice, Receipt)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ReportTypeName { get; set; }

        /// <summary>
        /// Description of the report type
        /// </summary>
        [MaxLength(256)]
        public string ReportTypeDescription { get; set; }
    }
}