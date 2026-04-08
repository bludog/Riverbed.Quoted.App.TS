using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    /**
    * Controller for managing company paintable items.
    * This controller provides CRUD operations for company paintable items.
    * The CompanyPaintableItem entity is used to represent a paintable item that a company can paint
    * and is used to store information about the item, such as its name, description, price, paint type, pricing type.
    * It is used in dropdowns to populate the PaintableItem field in the Room entity.
    */
    public class CompanyPaintableItem
    {
        [Key]
        public int Id { get; set; }

        // Name of the item (e.g., "Standard Closet", "Cabinet", etc.)
        public string Name { get; set; }

        // Detailed description of the item
        public string Description { get; set; }

        // PaintableItemCategoryId - Category of the item (e.g., "Cabinet", "House", etc.)
        public int PaintableItemCategoryId { get; set; }

        // Price associated with painting this item
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        // The type or quality of paint to be used (e.g., "SpeedHide", "SuperPaint", etc.)
        public int PaintTypeId { get; set; }

        // The type of pricing model to be used (e.g., "Per Item", "Per Square Foot", etc.)
        public int PricingTypeId { get; set; }

        // The total square footage of the item to be painted
        public double SquareFootage { get; set; }

        // The base estimated time required for painting the item (in hours, for example)
        public double BaseTime { get; set; }

        // Company Id
        public Guid CompanyId { get; set; }

    }
}
