// File: RoomSurfacePaintLayer.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    [Table("RoomSurfacePaintLayers", Schema = "dbo")]
    public class RoomSurfacePaintLayer
    {
        [Key]
        public int RoomSurfacePaintLayerId { get; set; }

        [Required]
        public int RoomSurfaceId { get; set; }

        /// <summary>
        /// Nullable until user selects a paint type.
        /// </summary>
        public int? PaintTypeId { get; set; }

        /// <summary>
        /// Stored as TINYINT in SQL. Aligns with PaintLayerType enum.
        /// </summary>
        [Required]
        public byte LayerType { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal Coats { get; set; } = 1m;

        [Column(TypeName = "decimal(5,4)")]
        public decimal WasteFactor { get; set; } = 0.1000m;

        [Column(TypeName = "decimal(12,4)")]
        public decimal GallonsCalculated { get; set; } = 0m;

        [Column(TypeName = "decimal(12,2)")]
        public decimal CostCalculated { get; set; } = 0m;

        public int SortOrder { get; set; } = 1;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

        // Navigation (optional)
        public RoomSurface? RoomSurface { get; set; }

        [ForeignKey(nameof(PaintTypeId))]
        public CompanyPaintType? CompanyPaintType { get; set; }

        // Convenience (not mapped)
        [NotMapped]
        public PaintLayerType LayerTypeEnum => (PaintLayerType)LayerType;
    }
}
