using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class ItemPaint
    {
        public int Id { get; set; }
        public int AreaItemId { get; set; }        
        public bool Active { get; set; }
        public int Gallons { get; set; }
        public int OverrideGallons { get; set; }
        public bool OverrideGallonsFlag { get; set; }
        public int Coats { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int PaintTypeId { get; set; }
        [JsonIgnore]
        public PaintType? PaintType { get; set; }       
    }
}
