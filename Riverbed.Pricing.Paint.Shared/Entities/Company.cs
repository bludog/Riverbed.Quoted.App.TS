using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class Company : Address
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyRepresentativeName { get; set; }
        public string PhoneNumber { get; set; }
        public bool isActive { get; set; } = false;
        public int? CompanySettingsId { get; set; } = 1;
        public int? CompanyDefaultsId { get; set; } = 1;
        public int? CompanyLogoDataFileId { get; set; } = 1;
        public string? CompanyLogoURL { get; set; } = string.Empty;
        public string? CompanyInsurancePdfUrl { get; set; } = string.Empty;
        public string? Email { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual List<CompanyCustomer>? CompanyCustomers { get; set; }
    }
}
