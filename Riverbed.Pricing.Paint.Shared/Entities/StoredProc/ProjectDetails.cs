using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities.StoredProc
{
    
    public class ProjectDetails
    {
        public int ProjectId { get; set; }
        public int CompanyCustomerId { get; set; }
        public string ProjectName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string ZipCode { get; set; }
        public int ProjectTotalSquareFootage { get; set; }
    }


}
