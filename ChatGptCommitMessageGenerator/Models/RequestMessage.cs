using Newtonsoft.Json;

namespace ChatGptCommitMessageGenerator.Models
{
    public class RequestMessage
    {
        [JsonProperty("role")] public string Role { get; set; }

        [JsonProperty("content")] public string Content { get; set; }
    }
}