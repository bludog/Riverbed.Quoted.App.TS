using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities.StoredProc
{
    public class CompanyCustomerDetails
    {
        public int CompanyCustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CompanyId { get; set; }
        public string CustomerAddress1 { get; set; }
        public string CustomerAddress2 { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerZipCode { get; set; }
        public string CustomerStateCode { get; set; }
        public string CustomerEmail { get; set; }
        public Guid CustomerId { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public string? SecondaryCcEmail { get; set; }
        public string? SecondaryPhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public int CompanySettingsId { get; set; }
        public int CompanyDefaultsId { get; set; }
        public string CompanyAddress1 { get; set; }
        public string CompanyAddress2 { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyZipCode { get; set; }
        public string CompanyStateCode { get; set; }
        public string CompanyEmail { get; set; }
    }

}
