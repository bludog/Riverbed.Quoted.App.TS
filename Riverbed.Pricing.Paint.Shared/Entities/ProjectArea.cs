using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class ProjectArea 
    {
        public int Id { get; set; }
        public string ProjectAreaName { get; set; }
        public string ProjectAreaNotes { get; set;}
        public bool IsChangeOrder { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsOptional { get; set; }
        public bool PrimeWalls { get; set; }
        public int NumberOfDoors { get; set; }
        public bool IncludeCeiling { get; set; }
        public bool IncludeBaseboards { get; set; }
        public bool IncludeCrownMoldings { get; set; }
        public float? AreaTotal { get; set; }
        public float? LaborCost { get; set; }
        public float? MaterialCost { get; set; }
        public int PaintQualityId { get; set; }    
        public int? PictureId { get; set; }
        public int ProjectDataId { get; set; }
 
      
    }
}
