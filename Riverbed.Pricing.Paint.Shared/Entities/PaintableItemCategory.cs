using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class PaintableItemCategory
    {
        public PaintableItemCategory()
        {
            // Initialize default values
            Name = string.Empty;
            Description = string.Empty;
        }
        public PaintableItemCategory(PaintableItemCategory item)
        {
            Id = item.Id;
            Name = item.Name;
            Description = item.Description;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
