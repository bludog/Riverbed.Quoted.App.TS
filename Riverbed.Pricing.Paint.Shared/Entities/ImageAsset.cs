using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    [Table("Images")]
    public class ImageAsset
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string ImageName { get; set; } = string.Empty;

        [Required]
        public byte[] ImageData { get; set; } = Array.Empty<byte>();

        [Required]
        [MaxLength(128)]
        public string ContentType { get; set; } = "application/octet-stream";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
