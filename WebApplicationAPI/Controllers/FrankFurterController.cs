using Microsoft.AspNetCore.Mvc;
using WebApplicationAPI.Services;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FrankFurterController : ControllerBase
    {
      
        private readonly ILogger<FrankFurterController> _logger;
        private readonly IFrankFurterService _frankFurterService;
        private readonly string[] _restrictedCurrencies = { "TRY", "PLN", "THB", "MXN" };

        public FrankFurterController(IFrankFurterService frankFurterService)
        {

           _frankFurterService = frankFurterService;
        }

        [HttpGet("LatesExchangeRates")]
        public async Task<IActionResult> GetAsync(string baseCurrency)
        {
            try
            {
                // Call the service to get exchange rates
                var exchangeRates = await _frankFurterService.GetLatesExchangeRatesAsync(baseCurrency);
                if (exchangeRates == null)
                {
                    return NotFound($"No exchange rate found for {baseCurrency}.");
                }
                // Return the response to the client
                return Ok(exchangeRates);
            }
            catch (Exception ex)
            {
                // Return an error if something goes wrong
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }


        }

        [HttpGet("ConvertCurrencies")]
        public async Task<IActionResult> GetRateCurrenciesAsync( string from,  string to,  decimal amount)
        {
            try
            {
                if (_restrictedCurrencies.Contains(to.ToUpper()))
                {
                    return BadRequest($"Currency conversion to {to} is not allowed.");
                }
                // Call the service to get exchange rates
                var convertedAmount = await _frankFurterService.ConvertCurrencyAsync(from,to,amount);
                if (convertedAmount == null)
                {
                    return NotFound($"Unable to perform conversion from {from} to {to}. No rates available.");
                }

                return Ok(new { from, to, amount, convertedAmount });
            }
            catch (Exception ex)
            {
                // Return an error if something goes wrong
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }


        }
        [HttpGet("historical")]
        public async Task<IActionResult> GetHistoricalRates(string basecurrency, [FromQuery] DateTime start,[FromQuery] DateTime end,[FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
           // add validation for the inputs

            try
            {
                var paginatedRates = await _frankFurterService.GetHistoricalRatesAsync(basecurrency, start, end, page, pageSize);

                if (paginatedRates == null)
                {
                    return NotFound("No historical rates found for the given criteria.");
                }

                return Ok(paginatedRates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
