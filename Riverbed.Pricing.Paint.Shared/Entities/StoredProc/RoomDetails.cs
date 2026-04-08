using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities.StoredProc
{
    public class RoomDetails
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public int Length { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public bool IsOptional { get; set; }
        public float TotalCost { get; set; }
        public int RoomSquareFootage { get; set; }
        public bool IncludeBaseboards { get; set; }
        public bool IncludeCeilings { get; set; }
        public bool IncludeCrownMoldings { get; set; }
        public bool IncludeDoors { get; set; }
        public bool IncludeWalls { get; set; }
        public bool IncludeWindows { get; set; }
    }

}
