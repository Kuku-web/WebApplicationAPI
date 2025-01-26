using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplicationAPI.Controllers;
using WebApplicationAPI.Model;
using WebApplicationAPI.Services;

namespace WebApplicationUnitTests
{
    public class FrankFurterControllerTests
    {
        private readonly Mock<IFrankFurterService> _mockService;
        private readonly FrankFurterController _controller;
        public FrankFurterControllerTests()
        {
            _mockService = new Mock<IFrankFurterService>();
            _controller = new FrankFurterController(_mockService.Object);
        }
        [Fact]
        public void Test1()
        {
          Assert.Equal(1, 1);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnOkResult_WhenExchangeRatesAreFound()
        {
            // Arrange
            var baseCurrency = "EUR";
            var exchangeRates = new Dictionary<string, decimal>
            {
                { "USD", 0.85m },
                { "GBP", 0.75m }
            };
            var expected = new FrankFurterResponse()
            {
                Base = baseCurrency,
                Rates = exchangeRates
            };
            _mockService.Setup(service => service.GetLatesExchangeRatesAsync(baseCurrency))
                        .ReturnsAsync(expected);

            // Act
            var result = await _controller.GetAsync(baseCurrency);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<FrankFurterResponse> (okResult.Value);
            Assert.Equal(expected, returnValue);
        }

        [Fact]
        public async Task GetRateCurrenciesAsync_ShouldReturnBadRequest_WhenCurrencyIsRestricted()
        {
            // Arrange
            var from = "EUR";
            var to = "TRY"; 
            var amount = 100m;

            // Act
            var result = await _controller.GetRateCurrenciesAsync(from, to, amount);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Currency conversion to TRY is not allowed.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetHistoricalRates_ShouldReturnOkResult_WhenHistoricalRatesAreFound()
        {
            // Arrange
            var baseCurrency = "USD";
            var startDate = DateTime.Parse("2021-01-01");
            var endDate = DateTime.Parse("2021-01-31");
            var rates = new Dictionary<string, decimal>
            {
                { "2021-01-01", 1.25m },
                { "2021-01-02", 1.26m }
            };
            var paginatedRates = new Dictionary<string, Dictionary<string, decimal>>
            {
                { "Rates_2025-01-01", rates }
            };

            var expected = new PaginatedResult<HistoricalFrankFurterResponse>()
            {
                Data = new HistoricalFrankFurterResponse()
                {
                    Base = baseCurrency,
                    StartDate = startDate,
                    EndDate = endDate,
                    Rates = paginatedRates,
                }
            };
            _mockService.Setup(service => service.GetHistoricalRatesAsync(baseCurrency, startDate, endDate, 1, 10))
                        .ReturnsAsync(expected);

            // Act
            var result = await _controller.GetHistoricalRates(baseCurrency, startDate, endDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PaginatedResult<HistoricalFrankFurterResponse>>(okResult.Value);
            Assert.Equal(expected, returnValue);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var baseCurrency = "EUR";

            // Setup the mock to throw an exception when calling the service method
            _mockService.Setup(service => service.GetLatesExchangeRatesAsync(baseCurrency))
                        .ThrowsAsync(new Exception("An unexpected error occurred"));

            // Act
            var result = await _controller.GetAsync(baseCurrency);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error: An unexpected error occurred", statusCodeResult.Value);
        }
    }
}