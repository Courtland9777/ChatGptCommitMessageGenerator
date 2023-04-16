using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;

namespace ChatGptCommitMessageGenerator.Helpers
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _httpClient;

        public HttpClientWrapper(HttpClient httpClient) => _httpClient = httpClient;

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content) =>
            _httpClient.PostAsync(requestUri, content);

        public AuthenticationHeaderValue Authorization
        {
            get => _httpClient.DefaultRequestHeaders.Authorization;
            set => _httpClient.DefaultRequestHeaders.Authorization = value;
        }
    }
}