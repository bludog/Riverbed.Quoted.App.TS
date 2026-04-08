// File: RoomSurface.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    [Table("RoomSurfaces", Schema = "dbo")]
    public class RoomSurface
    {
        [Key]
        public int RoomSurfaceId { get; set; }

        [Required]
        public int RoomId { get; set; }

        /// <summary>
        /// FK to dbo.SurfaceTypes.SurfaceTypeId (aligns with SurfaceType enum).
        /// Stored as TINYINT in SQL.
        /// </summary>
        [Required]
        public byte SurfaceType { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal AreaSqFt { get; set; } = 0m;

        [Column(TypeName = "decimal(12,2)")]
        public decimal? LinearFt { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Quantity { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

        // Navigation (optional)
        [ForeignKey(nameof(SurfaceType))]
        public SurfaceTypeLookup? SurfaceTypeLookup { get; set; }

        public ICollection<RoomSurfacePaintLayer> PaintLayers { get; set; } = new List<RoomSurfacePaintLayer>();

        // Convenience (not mapped)
        [NotMapped]
        public SurfaceType SurfaceTypeEnum => (SurfaceType)SurfaceType;
    }
}
