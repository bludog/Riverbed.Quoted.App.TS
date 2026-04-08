using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public bool IsChangeOrder { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsOptional { get; set; }
        public bool PrimeWalls { get; set; }
        public int NumberOfDoors { get; set; }
        public int NumberOfWindows { get; set; }
        public bool IncludeCeilings { get; set; }
        public bool IncludeBaseboards { get; set; }
        public bool IncludeCrownMoldings { get; set; }
        public bool IncludeWalls { get; set; }
        public bool IncludeDoors { get; set; }
        public bool IncludeWindows { get; set; }
        public float? AdditionalPrepTime { get; set; }
        public float? AreaTotal { get; set; }
        public float? LaborCost { get; set; } = 0;
        public float? MaterialCost { get; set; } = 0;
        public float? OverheadCost { get; set; } = 0;
        public float? Profit { get; set; } = 0;
        public int PaintQualityId { get; set; }
        public int? PictureId { get; set; }
        public int? RoomImageId { get; set; }
        public int ProjectDataId { get; set; }
        public Guid ProjectGuid { get; set; } = Guid.NewGuid();

        // Paint specifics per surface
        // Use your PaintType id reference if you have that entity
        public int? WallsPaintTypeId { get; set; }
        public int? WallsCoats { get; set; } = 2;

        public int? CeilingPaintTypeId { get; set; }
        public int? CeilingCoats { get; set; } = 2;

        public int? BaseboardsPaintTypeId { get; set; }
        public int? BaseboardsCoats { get; set; } = 2;

        public int? DoorsPaintTypeId { get; set; }
        public int? DoorsCoats { get; set; } = 2;

        public int? WindowsPaintTypeId { get; set; }
        public int? WindowsCoats { get; set; } = 2;

        // existing list of custom items
        public List<PaintableItem> PaintableItems { get; set; } = new List<PaintableItem>();
        // new list of surfaces with paint layers
        public ICollection<RoomSurface> RoomSurfaces { get; set; } = new List<RoomSurface>();

        public Room() { }

        // Copy constructor that accepts an EF Room entity and normalizes nulls into safe defaults
        public Room(Room? src)
        {
            if (src == null)
            {
                Id = 0;
                Name = string.Empty;
                Notes = string.Empty;
                PaintableItems = new List<PaintableItem>();
                return;
            }

            Id = src.Id;
            Name = src.Name ?? string.Empty;
            Notes = src.Notes ?? string.Empty;
            IsChangeOrder = src.IsChangeOrder;
            Length = src.Length;
            Width = src.Width;
            Height = src.Height;
            IsOptional = src.IsOptional;
            PrimeWalls = src.PrimeWalls;
            NumberOfDoors = src.NumberOfDoors;
            NumberOfWindows = src.NumberOfWindows;
            IncludeCeilings = src.IncludeCeilings;
            IncludeBaseboards = src.IncludeBaseboards;
            IncludeCrownMoldings = src.IncludeCrownMoldings;
            IncludeWalls = src.IncludeWalls;
            IncludeDoors = src.IncludeDoors;
            IncludeWindows = src.IncludeWindows;
            AdditionalPrepTime = src.AdditionalPrepTime;
            AreaTotal = src.AreaTotal;
            LaborCost = src.LaborCost ?? 0;
            MaterialCost = src.MaterialCost ?? 0;
            OverheadCost = src.OverheadCost ?? 0;
            Profit = src.Profit ?? 0;
            PaintQualityId = src.PaintQualityId;
            PictureId = src.PictureId;
            RoomImageId = src.RoomImageId;
            ProjectDataId = src.ProjectDataId;
            ProjectGuid = src.ProjectGuid;

            WallsPaintTypeId = src.WallsPaintTypeId;
            WallsCoats = src.WallsCoats ?? 1;
            CeilingPaintTypeId = src.CeilingPaintTypeId;
            CeilingCoats = src.CeilingCoats ?? 1;
            BaseboardsPaintTypeId = src.BaseboardsPaintTypeId;
            BaseboardsCoats = src.BaseboardsCoats ?? 1;
            DoorsPaintTypeId = src.DoorsPaintTypeId;
            DoorsCoats = src.DoorsCoats ?? 1;
            WindowsPaintTypeId = src.WindowsPaintTypeId;
            WindowsCoats = src.WindowsCoats ?? 1;

            PaintableItems = src.PaintableItems?.Select(pi => pi.Clone()).ToList() ?? new List<PaintableItem>();
        }

        public float TotalCost()
        {
            return (LaborCost ?? 0) + (MaterialCost ?? 0);
        }


        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Room r = (Room)obj;
            return (Id == r.Id)
                && (Name == r.Name)
                && (Notes == r.Notes)
                && (IsChangeOrder == r.IsChangeOrder)
                && (Length == r.Length)
                && (Width == r.Width)
                && (Height == r.Height)
                && (IsOptional == r.IsOptional)
                && (PrimeWalls == r.PrimeWalls)
                && (NumberOfDoors == r.NumberOfDoors)
                && (NumberOfWindows == r.NumberOfWindows)
                && (IncludeCeilings == r.IncludeCeilings)
                && (IncludeBaseboards == r.IncludeBaseboards)
                && (IncludeCrownMoldings == r.IncludeCrownMoldings)
                && (IncludeWalls == r.IncludeWalls)
                && (IncludeDoors == r.IncludeDoors)
                && (IncludeWindows == r.IncludeWindows)
                && (AdditionalPrepTime == r.AdditionalPrepTime)
                && (AreaTotal == r.AreaTotal)
                && (LaborCost == r.LaborCost)
                && (MaterialCost == r.MaterialCost)
                && (OverheadCost == r.OverheadCost)
                && (PaintQualityId == r.PaintQualityId)
                && (PictureId == r.PictureId)
                && (RoomImageId == r.RoomImageId)
                && (ProjectDataId == r.ProjectDataId)
                && (WallsPaintTypeId == r.WallsPaintTypeId)
                && (WallsCoats == r.WallsCoats)
                && (CeilingPaintTypeId == r.CeilingPaintTypeId)
                && (CeilingCoats == r.CeilingCoats)
                && (BaseboardsPaintTypeId == r.BaseboardsPaintTypeId)
                && (BaseboardsCoats == r.BaseboardsCoats)
                && (DoorsPaintTypeId == r.DoorsPaintTypeId)
                && (DoorsCoats == r.DoorsCoats)
                && (WindowsPaintTypeId == r.WindowsPaintTypeId)
                && (WindowsCoats == r.WindowsCoats);
        }

        public Room Clone()
        {
            return new Room
            {
                Id = Id,
                Name = Name,
                Notes = Notes,
                IsChangeOrder = IsChangeOrder,
                Length = Length,
                Width = Width,
                Height = Height,
                IsOptional = IsOptional,
                PrimeWalls = PrimeWalls,
                NumberOfDoors = NumberOfDoors,
                NumberOfWindows = NumberOfWindows,
                IncludeCeilings = IncludeCeilings,
                IncludeBaseboards = IncludeBaseboards,
                IncludeCrownMoldings = IncludeCrownMoldings,
                IncludeWalls = IncludeWalls,
                IncludeDoors = IncludeDoors,
                IncludeWindows = IncludeWindows,
                AdditionalPrepTime = AdditionalPrepTime,
                AreaTotal = AreaTotal,
                LaborCost = LaborCost,
                MaterialCost = MaterialCost,
                OverheadCost = OverheadCost,
                PaintQualityId = PaintQualityId,
                PictureId = PictureId,
                RoomImageId = RoomImageId,
                ProjectDataId = ProjectDataId,
                ProjectGuid = ProjectGuid,
                // new fields
                WallsPaintTypeId = WallsPaintTypeId,
                WallsCoats = WallsCoats,
                CeilingPaintTypeId = CeilingPaintTypeId,
                CeilingCoats = CeilingCoats,
                BaseboardsPaintTypeId = BaseboardsPaintTypeId,
                BaseboardsCoats = BaseboardsCoats,
                DoorsPaintTypeId = DoorsPaintTypeId,
                DoorsCoats = DoorsCoats,
                WindowsPaintTypeId = WindowsPaintTypeId,
                WindowsCoats = WindowsCoats,
                PaintableItems = PaintableItems.Select(pi => pi.Clone()).ToList()
            };
        }
    }
}
