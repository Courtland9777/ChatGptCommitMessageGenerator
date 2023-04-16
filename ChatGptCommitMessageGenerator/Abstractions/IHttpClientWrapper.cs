using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ChatGptCommitMessageGenerator.Abstractions
{
    public interface IHttpClientWrapper
    {
        AuthenticationHeaderValue Authorization { get; set; }
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
    }
}