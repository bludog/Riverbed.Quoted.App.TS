using System.ComponentModel.DataAnnotations;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class InvoiceStatus
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; } = string.Empty;
    }
}
