using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;

namespace Web
{
    public class UniversalApiManager
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UniversalApiManager(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET-запросы
        public async Task<T> GetAsync<T>(string clientName, string requestUri)
        {
            var client = _httpClientFactory.CreateClient(clientName);

            return await client.GetFromJsonAsync<T>(requestUri);
        }

        public async Task<T> GetAsync<T>(string clientName, string requestUri, string token)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await client.GetFromJsonAsync<T>(requestUri);
        }

        // POST-запросы
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string clientName, string requestUri, TRequest data)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            var response = await client.PostAsJsonAsync(requestUri, data);

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string clientName, string requestUri, TRequest data, string token)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsJsonAsync(requestUri, data);

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        // PUT-запросы
        public async Task<TResponse> PutAsync<TRequest, TResponse>(string clientName, string requestUri, TRequest data)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            var response = await client.PutAsJsonAsync(requestUri, data);

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<TResponse> PutAsync<TRequest, TResponse>(string clientName, string requestUri, TRequest data, string token)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsJsonAsync(requestUri, data);

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        // DELETE-запросы
        public async Task DeleteAsync(string clientName, string requestUri)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            var response = await client.DeleteAsync(requestUri);
        }

        public async Task DeleteAsync(string clientName, string requestUri, string token)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync(requestUri);
        }

        // PATCH-запросы
        public async Task<TResponse> PatchAsync<TRequest, TResponse>(string clientName, string requestUri, TRequest data)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri)
            {
                Content = content
            };
            var response = await client.SendAsync(request);

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<TResponse> PatchAsync<TRequest, TResponse>(string clientName, string requestUri, TRequest data, string token)
        {
            var client = _httpClientFactory.CreateClient(clientName);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri)
            {
                Content = content
            };
            var response = await client.SendAsync(request);

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
    }
}
