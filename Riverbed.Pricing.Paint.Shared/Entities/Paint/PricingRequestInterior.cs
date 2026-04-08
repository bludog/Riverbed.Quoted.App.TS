using Riverbed.Pricing.Paint.Shared.Entities;
using System.Text.Json.Serialization;

namespace Riverbed.Pricing.Paint.Shared
{
    public class PricingRequestInterior
    {
        public int Id { get; set; }
        public List<Room>? Rooms { get; set; }
        public Guid CompanyGuid { get; set; }
    }

   
   
}
