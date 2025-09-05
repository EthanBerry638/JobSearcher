using System.Text.Json.Serialization;

namespace JobFetcherManager
{
    public class AdzunaResponse
    {
        [JsonPropertyName("results")]
        public List<AdzunaJob>? Results { get; set; }
    }

    public class AdzunaJob
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("company")]
        public CompanyInfo? Company { get; set; }

        [JsonPropertyName("location")]
        public LocationInfo? Location { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("redirect_url")]
        public string? RedirectUrl { get; set; }

        [JsonPropertyName("created")]
        public string? Created { get; set; }
    }

    public class CompanyInfo
    {
        [JsonPropertyName("display_name")]
        public string? DisplayName { get; set; }
    }

    public class LocationInfo
    {
        [JsonPropertyName("display_name")]
        public string? DisplayName { get; set; }
    }
}