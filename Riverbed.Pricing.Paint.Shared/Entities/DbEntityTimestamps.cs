// File: DbEntityTimestamps.cs (Optional helper base class if you want it)
using System;

namespace Riverbed.Pricing.Paint.Shared.Entities
{
    public abstract class DbEntityTimestamps
    {
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
    }
}
