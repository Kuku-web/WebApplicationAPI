using Microsoft.Extensions.Caching.Memory;
using WebApplicationAPI.Helper;
using WebApplicationAPI.Model;
using WebApplicationAPI.Services;

namespace WebApplicationAPI.Controllers
{
    public class FrankFurterService : IFrankFurterService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
       // private const string ExchangeRatesCacheKey = "ExchangeRates";

        public FrankFurterService(IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }
        public FrankFurterResponse ConvertCurrency(string convertFrom, string convertTo)
        {
            throw new NotImplementedException();
        }

        public FrankFurterResponse GetHistoricalExchangeRates(string baseCurrency)
        {
            throw new NotImplementedException();
        }

        public async Task<FrankFurterResponse?> GetLatesExchangeRatesAsync(string baseCurrency)
        {
            if (_cache.TryGetValue(baseCurrency, out FrankFurterResponse? cachedRates))
            {
                return cachedRates;
            }

            var client = _httpClientFactory.CreateClient("FrankfurterClient");

            // Construct the API path with query parameters
            var response = await client.GetAsync($"/latest?base={baseCurrency}");

            if (response.IsSuccessStatusCode)
            {
                // Read the response content
                var jsonResponse = await response.Content.ReadFromJsonAsync<FrankFurterResponse>();
                var nextRefresh = TimeConverter.GetNextRefreshTimeCET();

                // Set the cache expiration time to the next refresh (16:00 CET)
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = nextRefresh
                };

                // Store the result in cache
                _cache.Set(baseCurrency, jsonResponse, cacheOptions);

                return jsonResponse;
            }

            // Handle non-success status codes (e.g., return null or throw an exception)
            throw new Exception("Error calling Frankfurter API");
        }


        public async Task<decimal?> ConvertCurrencyAsync(string from, string to, decimal amount)
        {
            // Get cached exchange rates
            var exchangeRates = await GetLatesExchangeRatesAsync(from);

            if (exchangeRates == null || !exchangeRates.Rates.Any(r => r.Key.ToUpper() == to.ToUpper()))
            {
                return null; // Return null if rates are not available
            }

            // Get the exchange rate
            var rate = exchangeRates.Rates.First(x => x.Key.ToUpper() == to.ToUpper()).Value;

            // Perform conversion
            var convertedAmount = amount * rate;

            return convertedAmount;
        }


        public async Task<PaginatedResult<HistoricalFrankFurterResponse>?> GetHistoricalRatesAsync(string baseCurrency, DateTime start, DateTime end, int page, int pageSize)
        {

            string cacheKey = $"HistoricalRates_{baseCurrency}_{start:yyyyMMdd}_{end:yyyyMMdd}";
            
            // Try to get cached historical rates
            if (!_cache.TryGetValue(cacheKey, out HistoricalFrankFurterResponse? historicalRates))
            {
                var client = _httpClientFactory.CreateClient("FrankfurterClient");

                var response = await client.GetAsync($"https://api.frankfurter.app/{start:yyyy-MM-dd}..{end:yyyy-MM-dd}?base={baseCurrency}");
                if (response.IsSuccessStatusCode)
                {

                    historicalRates = await response.Content.ReadFromJsonAsync<HistoricalFrankFurterResponse>();

                    if (historicalRates != null && historicalRates.Rates != null)
                    {
                        // Set the cache expiration time to the next refresh (16:00 CET)
                        var cacheOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpiration = TimeConverter.GetNextRefreshTimeCET()
                        };
                        // Cache the historical rates
                        _cache.Set(cacheKey, historicalRates, cacheOptions); // Cache for 1 hour
                    }
                    else
                    {
                        return null; // No data
                    }
                }
            }
            if (historicalRates == null) return null;
            // Paginate the historical rates
            var paginatedResult = PaginateRates(historicalRates, page, pageSize);

            return paginatedResult;
        }



        private static PaginatedResult<HistoricalFrankFurterResponse> PaginateRates(HistoricalFrankFurterResponse historicalRates, int page, int pageSize)
        {
            var totalRates = historicalRates.Rates?.Count;
            var skip = (page - 1) * pageSize;
            var pagedRates = new Dictionary<string, Dictionary<string, decimal>>();

            // Ensure pagination only iterates over the relevant data (dates)
            var paginatedKeys = historicalRates.Rates?.Keys.Skip(skip).Take(pageSize);

            foreach (var key in paginatedKeys)
            {
                // Add the date and corresponding currency-to-rate mapping
                pagedRates.Add(key, historicalRates.Rates[key]);
            };
            return new PaginatedResult<HistoricalFrankFurterResponse>
            {
                TotalCount = totalRates,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRates / pageSize),
                Data = new HistoricalFrankFurterResponse
                {
                    Base = historicalRates.Base,
                    StartDate = historicalRates.StartDate,
                    Rates = pagedRates
                }
            };
        }
    }
}
