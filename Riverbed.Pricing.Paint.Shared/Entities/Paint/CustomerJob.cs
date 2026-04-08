using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities.Paint
{
    public class CustomerJob
    {
        public int Id { get; set; }
        public string JobName { get; set; }
        public string JobDescription { get; set; }
        public int? CompanyCustomerId { get; set; }
        public virtual CompanyCustomer? CompanyCustomer { get; set; }
        public int? PricingRequestInteriorId { get; set; }
        public virtual PricingRequestInterior? PricingRequestInterior { get; set; }
        public int? PricingRequestExteriorId { get; set; }
        public virtual PricingRequestExterior? PricingRequestExterior { get; set; }
    }
}
