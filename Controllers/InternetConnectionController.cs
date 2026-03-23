using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SystemControlApp.Controllers
{
    public static class InternetConnectionController
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        public static async Task<bool> IsConnectedToGoogleAsync()
        {
            try
            {
                using var response = await _httpClient.GetAsync(
                    "https://www.google.com",
                    HttpCompletionOption.ResponseHeadersRead);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
