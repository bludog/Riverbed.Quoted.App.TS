using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class AreaItemPricingResult
    {
        public int Id { get; set; }
        public float MaterialCost { get; set; }
        public float LaborCost { get; set; }
        public int AreaItemId { get; set; }
        public AreaItem? AreaItem { get; set; }
    }
}
