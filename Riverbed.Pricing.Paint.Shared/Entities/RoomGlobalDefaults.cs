using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class RoomGlobalDefaults
    {
        public int Id { get; set; }    
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int NumberOfDoors { get; set; }
        public bool IncludeCeilings { get; set; }
        public bool IncludeBaseboards { get; set; }
        public bool IncludeCrownMoldings { get; set; }
        public bool IncludeWalls { get; set; }
        public bool IncludeDoors { get; set; }
        public bool IncludeWindows { get; set; }
        public int PaintQualityId { get; set; }
        [JsonIgnore]
        public AllPaintQuality? PaintQuality { get; set; }       
        public Guid CompanyId { get; set; }
    }
}
