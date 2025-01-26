namespace WebApplicationAPI.Model
{
    public class FrankFurterResponse
    {
        public string? Base { get; set; } 
        public DateTime Date { get; set; } 
        public Dictionary<string, decimal>? Rates { get; set; }
    }
}
