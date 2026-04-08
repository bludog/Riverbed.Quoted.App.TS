using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class PaintType
    {
        public int Id { get; set; }
        public string PaintTypeName { get; set; }
        public int CoverageOneCoatSqFt { get; set; }
        public int CoverageTwoCoatsSqFt { get; set; }
        public float PricePerGallon { get; set; }
        public int PaintSheenId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual PaintSheen? PaintSheen { get; set; }
        public int PaintBrandId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual PaintBrand? PaintBrand { get; set; }
    }
}
