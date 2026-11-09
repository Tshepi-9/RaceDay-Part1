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

        private void AddApiCookie(HttpRequestMessage request)
        {
            var context = _httpContextAccessor.HttpContext;

            var apiCookie =
                context?.Session.GetString("ApiSessionCookie");

            if (!string.IsNullOrEmpty(apiCookie))
            {
                request.Headers.TryAddWithoutValidation(
                    "Cookie", apiCookie);
            }
        }

        private void SaveApiCookie(HttpResponseMessage response)
        {
            var context = _httpContextAccessor.HttpContext;

            if (response.Headers.TryGetValues(
                "Set-Cookie", out var cookies))
            {
                foreach (var cookie in cookies)
                {
                    var cookiePair = cookie.Split(';')[0];

                    if (cookiePair.StartsWith(
                        ".AspNetCore.Session=",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        var cookieValue =
                            cookiePair.Split('=', 2)[1];

                        if (string.IsNullOrEmpty(cookieValue))
                        {
                            context?.Session.Remove(
                                "ApiSessionCookie");
                        }
                        else
                        {
                            context?.Session.SetString(
                                "ApiSessionCookie", cookiePair);
                        }
                    }
                }
            }
        }

        // Send normal JSON requests.
        public async Task<HttpResponseMessage> SendAsync(
            HttpMethod method,
            string endpoint,
            object? body = null)
        {
            var request =
                new HttpRequestMessage(method, endpoint);

            AddApiCookie(request);

            if (body != null)
            {
                request.Content = JsonContent.Create(body);
            }

            var response =
                await _httpClient.SendAsync(request);

            SaveApiCookie(response);

            return response;
        }

        // Send image files using multipart/form-data.
        public async Task<HttpResponseMessage> SendFileAsync(
            string endpoint,
            IFormFile file)
        {
            using var content = new MultipartFormDataContent();

            using var stream = file.OpenReadStream();

            var fileContent = new StreamContent(stream);

            fileContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(
                    file.ContentType);

            content.Add(fileContent, "file", file.FileName);

            using var request =
                new HttpRequestMessage(HttpMethod.Post, endpoint);

            AddApiCookie(request);

            request.Content = content;

            var response =
                await _httpClient.SendAsync(request);

            SaveApiCookie(response);

            return response;
        }

        // Read JSON responses.
        public async Task<T?> ReadAsync<T>(
            HttpResponseMessage response)
        {
            return await response.Content.ReadFromJsonAsync<T>(
                _jsonOptions);
        }
    }
}