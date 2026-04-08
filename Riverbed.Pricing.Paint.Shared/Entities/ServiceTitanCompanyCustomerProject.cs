using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class ServiceTitanCompanyCustomerProject
    {
        public int Id { get; set; }
        public Guid CompanyCustomerGuid { get; set; }       
        public int? ServiceTitanCustomerId { get; set; }
        public Guid ProjectGuid { get; set; }
        public int? ServiceTitanProjectId { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
    }
}
