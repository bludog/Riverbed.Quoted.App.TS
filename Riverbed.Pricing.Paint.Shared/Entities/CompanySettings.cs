using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class CompanySettings
    {
        public int Id { get; set; }
        public float HourlyRate { get; set; }
        public float OverheadPercentage { get; set; }
        public float LaborPercentage { get; set; }
        public float MaterialPercentage { get; set; }
        public Guid? CompanyId { get; set; }
    }
}
