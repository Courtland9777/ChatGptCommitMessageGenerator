using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ChatGptCommitMessageGenerator.Abstractions;
using ChatGptCommitMessageGenerator.Exceptions;
using Newtonsoft.Json;

namespace ChatGptCommitMessageGenerator.Helpers
{
    public static class GenerateMessageHelpers
    {
        private const int TooManyRequestsStatusCode = 429;

        public static async Task<string> GetChatGptCommitMessageAsync(IHttpClientWrapper client, string gitDiff)
        {
            const string apiUrl = "https://api.openai.com/v1/chat/completions";
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            var request = new Request
            {
                Messages = new[]
                {
                    new RequestMessage
                    {
                        Role = "system",
                        Content =
                            "You are a helpful assistant that can summarize git diffs and suggest commit messages."
                    },
                    new RequestMessage
                    {
                        Role = "user",
                        Content =
                            $"In one sentence, create a concise and informative commit message that summarizes the following diff:{Environment.NewLine}{gitDiff}{Environment.NewLine}Commit message:"
                    }
                }
            };

            var requestData = JsonConvert.SerializeObject(request);
            var content = new StringContent(requestData, Encoding.UTF8, "application/json");


            client.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            var httpResponseMessage = await client.PostAsync(apiUrl, content);

            if (!httpResponseMessage.IsSuccessStatusCode)
                throw new Exception($"Error: {httpResponseMessage.StatusCode} - {httpResponseMessage.ReasonPhrase}");
            var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Response>(responseString);
            return response.Choices[0].Message.Content;
        }


        public static async Task<string> GetChatGptCommitMessageWithRetryAsync(IHttpClientWrapper httpClientWrapper,
            string diff,
            int maxTokens = 100, string apiVersion = "v1", string engine = "gpt-3.5-turbo", int maxRetries = 5)
        {
            var retryCount = 0;
            string result = null;
            const int waitTime = 3000;

            while (retryCount < maxRetries)
                try
                {
                    result = await GetChatGptCommitMessageAsync(httpClientWrapper, diff);
                    break; // Exit the loop if the API call is successful
                }
                catch (ApiException ex)
                {
                    if (ex.StatusCode == (HttpStatusCode)TooManyRequestsStatusCode)
                    {
                        // Wait for a fixed amount of time before retrying
                        await Task.Delay(waitTime);
                        retryCount++;
                    }
                    else
                    {
                        // Rethrow the exception if it's not a rate limit error
                        throw;
                    }
                }

            if (retryCount == maxRetries)
                throw new ApiException((HttpStatusCode)TooManyRequestsStatusCode,
                    "Failed to make API call after maximum retries due to rate limit.");

            return result;
        }
    }
    //$"In one sentence, create a concise and informative commit message that summarizes the following diff:{Environment.NewLine}{diff}{Environment.NewLine}Commit message:"

    public class Request
    {
        [JsonPropertyName("model")] public string Model { get; set; } = "gpt-3.5-turbo";

        [JsonPropertyName("max_tokens")] public int MaxTokens { get; set; } = 4000;

        [JsonPropertyName("messages")] public RequestMessage[] Messages { get; set; }
    }

    public class RequestMessage
    {
        [JsonPropertyName("role")] public string Role { get; set; }

        [JsonPropertyName("content")] public string Content { get; set; }
    }

    public class Response
    {
        [JsonPropertyName("id")] public string Id { get; set; }

        [JsonPropertyName("created")] public int Created { get; set; }

        [JsonPropertyName("model")] public string Model { get; set; }

        [JsonPropertyName("usage")] public ResponseUsage Usage { get; set; }

        [JsonPropertyName("choices")] public ResponseChoice[] Choices { get; set; }
    }

    public class ResponseUsage
    {
        [JsonPropertyName("prompt_tokens")] public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")] public int TotalTokens { get; set; }
    }

    public class ResponseChoice
    {
        [JsonPropertyName("message")] public ResponseMessage Message { get; set; }

        [JsonPropertyName("finish_reason")] public string FinishReason { get; set; }

        [JsonPropertyName("index")] public int Index { get; set; }
    }

    public class ResponseMessage
    {
        [JsonPropertyName("role")] public string Role { get; set; }

        [JsonPropertyName("content")] public string Content { get; set; }
    }
}