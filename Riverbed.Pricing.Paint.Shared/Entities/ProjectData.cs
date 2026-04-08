using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class ProjectData
    {
        public int Id { get; set; }
        [Required]
        public string ProjectName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string CustomerEmail { get; set; }
       
        public string CustomerPhone { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [MaxLength(256)]
        public string? SecondaryCcEmail { get; set; }
        [Phone]
        [MaxLength(32)]
        public string? SecondaryPhoneNumber { get; set; }
        [Required]
        public string Address1 { get; set; }
        public string? Address2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string StateCode { get; set; }
        [Required]
        public string ZipCode { get; set; }
        public int StatusCodeId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public StatusCode? StatusCode { get; set; }
        public List<Adjustment>? Adjustments { get; set; } = new List<Adjustment>();
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BaseAmount { get; set; }
        public decimal TotalPrice
        {
            get
            {
                decimal total = BaseAmount;
                if (Adjustments != null)
                {
                    foreach (var adjustment in Adjustments)
                    {
                        total = adjustment.Apply(total);
                    }
                }
                return total;
            }
        }
        public string Summary { get; set; }
        public string ScopeOfWork { get; set; }
        public int ServiceTitanId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime CompletedDate { get; set; }
        public int CompanyCustomerId { get; set; }
        public Guid CompanyCustomerGuidId { get; set; } = Guid.NewGuid();
        public Guid ProjectGuid { get; set; } = Guid.NewGuid();
        [System.Text.Json.Serialization.JsonIgnore]
        public CompanyCustomer? CompanyCustomer { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual List<ProjectArea>? ProjectAreas { get; set; }

        /// <summary>
        /// Normalizes optional fields by converting empty strings to null to prevent validation errors
        /// </summary>
        public void NormalizeOptionalFields()
        {
            if (string.IsNullOrWhiteSpace(SecondaryCcEmail))
                SecondaryCcEmail = null;

            if (string.IsNullOrWhiteSpace(SecondaryPhoneNumber))
                SecondaryPhoneNumber = null;

            if (string.IsNullOrWhiteSpace(Address2))
                Address2 = null;
        }
    }
}
