using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities.StoredProc
{
    public class CompanyDetails
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public int CompanySettingsId { get; set; }
        public int CompanyDefaultsId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string StateCode { get; set; }
        public string Email { get; set; }
    }

}
