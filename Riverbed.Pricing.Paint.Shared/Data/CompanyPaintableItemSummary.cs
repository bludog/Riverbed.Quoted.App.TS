using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Data
{
    public class CompanyPaintableItemSummary
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CompanyPaintableItemId { get; set; }

        // Name of the item (e.g., "Standard Closet", "Cabinet", etc.)
        public string Name { get; set; }

        // Detailed description of the item
        public string Description { get; set; }

        // Paintable Item Category Id (Id = 7 is Other)
        public int PaintableItemCategoryId { get; set; }
    }
}
