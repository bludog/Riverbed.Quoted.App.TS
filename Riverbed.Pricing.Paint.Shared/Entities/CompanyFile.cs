using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class CompanyFile
    {
        public int Id { get; set; }
        public Guid CompanyGuid { get; set; }
        public int DataFileId { get; set; }
    }
}
