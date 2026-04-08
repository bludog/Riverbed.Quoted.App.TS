using System;

namespace Riverbed.Pricing.Paint.Shared.Services
{
    public class ImageUploadResult
    {
        public Guid Id { get; set; }
        public string MetadataUrl { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty;
    }
}
