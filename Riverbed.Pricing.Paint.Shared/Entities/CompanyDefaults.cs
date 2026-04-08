using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class CompanyDefaults
    {
        public int Id { get; set; }
        public bool Baseboards { get; set; }
        public bool Ceilings { get; set; }
        public bool Doors { get; set; }
        public bool Walls { get; set; }
        public bool Windows { get; set; }
        public bool TrimDoors { get; set; }
        public int PaintTypeCeilingsId { get; set; }
        public int PaintTypeWallsId { get; set; }
        public int PaintTypeBaseboardsId { get; set; }
        public int PaintTypeTrimDoorsId { get; set; }
        public int PaintTypeWindowsId { get; set; }
        public Guid? CompanyId { get; set; }
    }
}
