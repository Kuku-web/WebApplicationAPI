using Polly.Extensions.Http;
using Polly;
using System.Runtime.CompilerServices;

namespace WebApplicationAPI
{
    public static class ServiceExtensions
    {
        public static void AddFrankFurterHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient("FrankfurterClient", client =>
             {
                 client.BaseAddress = new Uri("https://api.frankfurter.app");
                 client.DefaultRequestHeaders.Add("Accept", "application/json");
             }).AddPolicyHandler(GetRetryPolicy());       // Retry policy

        }
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

    }
}
