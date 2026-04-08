using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities.StoredProc
{
    public class ProjectComplexDetails
    {
        public ProjectDetails ProjectDetails { get; set; }
        public List<RoomDetails> RoomDetails { get; set; }
        public CompanyCustomerDetails CompanyCustomerDetails { get; set; }
        public CompanyDetails CompanyDetails { get; set; }
    }

}
