using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class AreaItemCalculationResult
    {
        public int Id { get; set; }
        public float MaterialNeeded { get; set; }
        public float TimeNeeded { get; set; }
        public int TotalArea { get; set; }
        public int AreaItemId { get; set; }
        public AreaItem? AreaItem { get; set; }
    }
}
