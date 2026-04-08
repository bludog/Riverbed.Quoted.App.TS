using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class AreaItem
    {
        public int Id { get; set; }
        public string AreaItemName { get; set; }
        public float AreaItemPrice { get; set; }
        public int ItemCount { get; set; }
        // Hour, Item, SqFt, LinearFt
        public int PricingTypeId { get; set; }
        public PricingType? PricingType { get; set; }
        // Wall, Ceiling, Trim, Door, Window, Floor
        public int ItemTypeId { get; set; }
        public ItemType? ItemType { get; set; }
        // Easy, Medium, Hard
        public int DifficultyLevelId { get; set; }
        public DifficultyLevel? DifficultyLevel { get; set; }
        // PaintType, PaintBrand, PaintSheen, PaintTypeName
        public int? PaintTypeId { get; set; }
        public ItemPaint? ItemPaint { get; set; }        
        public int? ProjectAreaId { get; set; }
        public ProjectArea? ProjectArea { get; set; }
        public float MaterialNeeded { get; set; } = 0.0f;
        public float TimeNeeded { get; set; } = 0.0f;
        public int TotalArea { get; set; } = 0;
        public float MaterialCost { get; set; } = 0.0f;
        public float LaborCost { get; set; } = 0.0f;

    }
}
