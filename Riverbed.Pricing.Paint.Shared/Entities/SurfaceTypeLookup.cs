// File: SurfaceTypeLookup.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    [Table("SurfaceTypes", Schema = "dbo")]
    public class SurfaceTypeLookup
    {
        [Key]
        [Column("SurfaceTypeId")]
        public byte SurfaceTypeId { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        public bool IsLinear { get; set; } = false;

        public bool IsAreaBased { get; set; } = true;

        public int SortOrder { get; set; }

        // Navigation
        public ICollection<RoomSurface> RoomSurfaces { get; set; } = new List<RoomSurface>();
    }
}
