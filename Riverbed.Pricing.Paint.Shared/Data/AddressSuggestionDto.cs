
using System.Text.Json.Serialization;

namespace Riverbed.Pricing.Paint.Shared.Data;

public class AddressSuggestionDto
{
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("place_id")]
    public string PlaceId { get; set; }
}


public class GoogleAutocompleteResponse
{
    [JsonPropertyName("predictions")]
    public List<AddressSuggestionDto> Predictions { get; set; }
}


public class AddressDetailsDto
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public string FormattedAddress { get; set; }
}