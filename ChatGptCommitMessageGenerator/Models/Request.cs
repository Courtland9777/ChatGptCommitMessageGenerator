using Newtonsoft.Json;

namespace ChatGptCommitMessageGenerator.Models
{
    public class Request
    {
        [JsonProperty("model")] public string Model { get; set; } = "gpt-3.5-turbo";
        [JsonProperty("messages")] public RequestMessage[] Messages { get; set; }
        [JsonProperty("temperature")] public double Temperature { get; set; } = 0.7;
        [JsonProperty("max_tokens")] public int MaxTokens { get; set; } = 2000;
    }
}