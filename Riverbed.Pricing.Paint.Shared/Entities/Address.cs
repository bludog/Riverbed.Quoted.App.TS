using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class Address
    {
        [Required]
        [MaxLength(100)]
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;
        [Required]
        [MaxLength(10)]
        public string ZipCode { get; set; } = string.Empty;
        [Required]
        [MaxLength(32)]
        public string StateCode { get; set; } = string.Empty;
    }
}
