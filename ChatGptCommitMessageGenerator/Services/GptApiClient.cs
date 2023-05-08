using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;
using ChatGptCommitMessageGenerator.Exceptions;
using ChatGptCommitMessageGenerator.Models;
using Newtonsoft.Json;

namespace ChatGptCommitMessageGenerator.Services
{
    public class GptApiClient : IGptApiClient
    {
        private const int TooManyRequestsStatusCode = 429;
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
        private readonly HttpClient _httpClient;

        public GptApiClient(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<Response> PostAsync(Request requestData, int maxRetries = 5,
            int initialWaitTimeMilliseconds = 1000)
        {
            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8,
                "application/json");

            HttpResponseMessage httpResponseMessage = default;
            var retryCount = 0;

            while (retryCount < maxRetries)
            {
                httpResponseMessage = await _httpClient.PostAsync(ApiUrl, content).ConfigureAwait(false);

                if (httpResponseMessage.IsSuccessStatusCode ||
                    httpResponseMessage.StatusCode != (HttpStatusCode)TooManyRequestsStatusCode) break;

                // Calculate the exponential back-off time
                var waitTime = initialWaitTimeMilliseconds * (1 << retryCount);

                // Wait for the specified amount of time before retrying
                await Task.Delay(waitTime);

                retryCount++;
            }

            await EnsureSuccessAsync(httpResponseMessage).ConfigureAwait(false);

            if (httpResponseMessage == null) throw new NullReferenceException();

            var responseString = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<Response>(responseString);
            return response;
        }

        private static async Task EnsureSuccessAsync(HttpResponseMessage httpResponseMessage)
        {
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                if (httpResponseMessage.Content == null)
                    throw new GptApiException(httpResponseMessage.StatusCode,
                        $"Error: {httpResponseMessage.StatusCode}");
                var errorMessage = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new GptApiException(httpResponseMessage.StatusCode,
                    $"Error: {httpResponseMessage.StatusCode} - {httpResponseMessage.ReasonPhrase}\n{errorMessage}");
            }
        }
    }
}