using Microsoft.AspNetCore.Mvc;
using Riverbed.Pricing.Paint.Shared.Data;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Riverbed.Pricing.Paint.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GooglePlacesController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _googleApiKey = "AIzaSyD2syuU-Xs_EZSEge_PWTIVjW30y-6SNsM"; // Replace after build

    public GooglePlacesController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet("autocomplete")]
    public async Task<IActionResult> Autocomplete([FromQuery] string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return BadRequest();

        var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={HttpUtility.UrlEncode(input)}&types=address&key={_googleApiKey}";
        var response = await _httpClient.GetAsync(url);
        var jsonString = await response.Content.ReadAsStringAsync();
        var suggestions = MapGoogleJsonToSuggestions(jsonString);

        var json = JsonSerializer.Serialize(suggestions);
        return Content(json, "application/json");
    }

    [HttpGet("details")]
    public async Task<IActionResult> Details([FromQuery] string placeId)
    {
        if (string.IsNullOrWhiteSpace(placeId))
            return BadRequest();

        var url = $"https://maps.googleapis.com/maps/api/place/details/json?placeid={HttpUtility.UrlEncode(placeId)}&key={_googleApiKey}";
        var response = await _httpClient.GetAsync(url);
        var jsonString = await response.Content.ReadAsStringAsync();
        var address = MapGoogleJsonToAddressDetails(jsonString);
        var json = JsonSerializer.Serialize(address);
        return Content(json, "application/json");
    }

    public static List<AddressSuggestionDto> MapGoogleJsonToSuggestions(string json)
    {
        var response = JsonSerializer.Deserialize<GoogleAutocompleteResponse>(json);
        return response?.Predictions.ToList();        
    }

    public static AddressDetailsDto MapGoogleJsonToAddressDetails(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var result = doc.RootElement.GetProperty("result");
        var components = result.GetProperty("address_components");

        string streetNumber = null, route = null, city = null, state = null, postalCode = null, country = null;

        foreach (var comp in components.EnumerateArray())
        {
            var types = comp.GetProperty("types").EnumerateArray().Select(x => x.GetString()).ToList();
            if (types.Contains("street_number"))
                streetNumber = comp.GetProperty("long_name").GetString();
            else if (types.Contains("route"))
                route = comp.GetProperty("long_name").GetString();
            else if (types.Contains("locality") || types.Contains("sublocality_level_1") || types.Contains("sublocality"))
                city = comp.GetProperty("long_name").GetString();
            else if (types.Contains("administrative_area_level_1"))
                state = comp.GetProperty("short_name").GetString();
            else if (types.Contains("postal_code"))
                postalCode = comp.GetProperty("long_name").GetString();
            else if (types.Contains("country"))
                country = comp.GetProperty("long_name").GetString();
        }

        var formattedAddress = result.GetProperty("formatted_address").GetString();

        return new AddressDetailsDto
        {
            Street = $"{streetNumber} {route}".Trim(),
            City = city,
            State = state,
            PostalCode = postalCode,
            Country = country,
            FormattedAddress = formattedAddress
        };
    }
}