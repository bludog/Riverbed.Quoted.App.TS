using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Riverbed.Pricing.Paint.Shared.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    /**
    * PaintableItem entity is used to represent a paintable item that a company can paint
    * and is used to store information about the item, such as its name, description, price, paint type, pricing type.
    * It is populated using the CompanyPaintableItem entity and added to the Room entity.
    */
    public class PaintableItem
    {
        public PaintableItem()
        {
            // Initialize default values
            SquareFootage = 0;
            BaseTime = 0;
            AdditionalTime = 0;
            Count = 1;
            Coats = 1;
        }

        public PaintableItem(PaintableItem item)
        {
            Id = item.Id;
            Name = item.Name;
            Description = item.Description;
            Price = item.Price;
            PaintTypeId = item.PaintTypeId;
            PricingTypeId = item.PricingTypeId;
            SquareFootage = item.SquareFootage;
            BaseTime = item.BaseTime;
            AdditionalTime = item.AdditionalTime;
            Count = item.Count;
            Coats = item.Coats;
            RoomId = item.RoomId;
        }

        [Key]
        public int Id { get; set; }

        // Name of the item (e.g., "Standard Closet", "Cabinet", etc.)
        public string Name { get; set; }

        // Detailed description of the item
        public string Description { get; set; }

        // Price associated with painting this item
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        // The type or quality of paint to be used (e.g., "SpeedHide", "SuperPaint", etc.)
        public int PaintTypeId { get; set; }
        //[NotMapped]
        //public PaintType PaintType { get; set; }

        // The type of pricing model to be used (e.g., "Per Item", "Per Square Foot", etc.)
        public int PricingTypeId { get; set; }
        //[NotMapped]
        //public PricingType PricingType { get; set; }

        // The total square footage of the item to be painted
        public double SquareFootage { get; set; }

        // The base estimated time required for painting the item (in hours, for example)
        public double BaseTime { get; set; }

        // Any additional time required (e.g., for intricate details or prep work)
        public double AdditionalTime { get; set; }

        // The number of these items present (e.g., multiple cabinets)
        public int Count { get; set; }

        // Coats of paint to be applied
        public int Coats { get; set; }

        // Foreign key to associate the item with a specific room
        public int RoomId { get; set; }
        //[NotMapped]
        //public Room Room { get; set; }

        public int PaintableItemCategoryId { get; set; } // Category of the item (e.g., "Cabinet", "House", etc.)

        // Add this method
        public PaintableItem Clone()
        {
            return new PaintableItem
            {
                Id = Id,
                Name = Name,
                Description = Description,
                Price = Price,
                PaintTypeId = PaintTypeId,
                //PaintType = PaintType,
                PricingTypeId = PricingTypeId,
                //PricingType = PricingType,
                SquareFootage = SquareFootage,
                BaseTime = BaseTime,
                AdditionalTime = AdditionalTime,
                Count = Count,
                Coats = Coats,
                RoomId = RoomId,
                //Room = Room
            };
        }

        // Create a new PaintableItem using the values from a CompanyPaintableItem
        public static PaintableItem FromCompanyPaintableItem(CompanyPaintableItem companyPaintableItem, int roomId)
        {
            return new PaintableItem
            {
                Name = companyPaintableItem.Name,
                Description = companyPaintableItem.Description,
                Price = companyPaintableItem.Price,
                PaintTypeId = companyPaintableItem.PaintTypeId,
                PricingTypeId = companyPaintableItem.PricingTypeId,
                SquareFootage = companyPaintableItem.SquareFootage,
                BaseTime = companyPaintableItem.BaseTime,
                AdditionalTime = 0,
                Count = 1,
                Coats = 1,
                RoomId = roomId
            };
        }

       
    }
}
