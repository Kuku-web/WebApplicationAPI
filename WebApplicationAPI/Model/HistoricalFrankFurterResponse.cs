using System.Text.Json.Serialization;

namespace WebApplicationAPI.Model
{
    public class HistoricalFrankFurterResponse
    {
        public string? Base { get; set; }
        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime EndDate { get; set; }
        public Dictionary<string, Dictionary<string, decimal>>? Rates { get; set; }
    }
}
