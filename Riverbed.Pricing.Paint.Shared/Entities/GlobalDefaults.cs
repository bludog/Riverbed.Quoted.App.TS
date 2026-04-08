using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class GlobalDefaults
    {
        public int Id { get; set; }
        public float HourlyRate { get; set; }
        public float TimePerSquareFootWall { get; set; }
        public float TimePerSquareFootCeiling { get; set; }
        public float TimePerSquareFootBaseboard { get; set; }
        public float TimePerSquareFootDoor { get; set; }
    }
}
