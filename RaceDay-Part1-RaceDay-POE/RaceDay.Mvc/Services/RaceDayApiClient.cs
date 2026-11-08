using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.Mvc.Services
{
    public class RaceDayApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

        public RaceDayApiClient(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        // Send requests to the API
        public async Task<HttpResponseMessage> SendAsync(
            HttpMethod method,
            string endpoint,
            object? body = null)
        {
            var request = new HttpRequestMessage(method, endpoint);

            var context = _httpContextAccessor.HttpContext;

            // Forward the API session cookie
            var apiCookie = context?.Session.GetString("ApiSessionCookie");

            if (!string.IsNullOrEmpty(apiCookie))
            {
                request.Headers.TryAddWithoutValidation(
                    "Cookie",
                    apiCookie);
            }

            // Attach request body when supplied
            if (body != null)
            {
                request.Content = JsonContent.Create(body);
            }

            var response = await _httpClient.SendAsync(request);

            // Capture the API session cookie after login
            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                foreach (var cookie in cookies)
                {
                    var cookiePair = cookie.Split(';')[0];

                    if (cookiePair.StartsWith(
                        ".AspNetCore.Session=",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        var cookieValue = cookiePair.Split('=', 2)[1];

                        if (string.IsNullOrEmpty(cookieValue))
                        {
                            context?.Session.Remove("ApiSessionCookie");
                        }
                        else
                        {
                            context?.Session.SetString(
                                "ApiSessionCookie",
                                cookiePair);
                        }
                    }
                }
            }

            return response;
        }

        // Read API responses
        public async Task<T?> ReadAsync<T>(
            HttpResponseMessage response)
        {
            return await response.Content.ReadFromJsonAsync<T>(
                _jsonOptions);
        }
    }
}