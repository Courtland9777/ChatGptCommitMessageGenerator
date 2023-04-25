using Newtonsoft.Json;

namespace ChatGptCommitMessageGenerator.Models
{
    public class Response
    {
        [JsonProperty("id")] public string Id { get; set; }

        [JsonProperty("created")] public int Created { get; set; }

        [JsonProperty("model")] public string Model { get; set; }

        [JsonProperty("usage")] public ResponseUsage Usage { get; set; }

        [JsonProperty("choices")] public ResponseChoice[] Choices { get; set; }
    }
}