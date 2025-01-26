using WebApplicationAPI.Model;

namespace WebApplicationAPI.Services
{
    public interface IFrankFurterService
    {
        public Task<FrankFurterResponse?> GetLatesExchangeRatesAsync(string baseCurrency);
        public Task<decimal?> ConvertCurrencyAsync(string from, string to, decimal amount);
        public Task<PaginatedResult<HistoricalFrankFurterResponse>?> GetHistoricalRatesAsync(string baseCurrency, DateTime start, DateTime end, int page, int pageSize);
    }
}
