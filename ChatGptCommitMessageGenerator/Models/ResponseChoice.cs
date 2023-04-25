using Newtonsoft.Json;

namespace ChatGptCommitMessageGenerator.Models
{
    public class ResponseChoice
    {
        [JsonProperty("message")] public ResponseMessage Message { get; set; }

        [JsonProperty("finish_reason")] public string FinishReason { get; set; }

        [JsonProperty("index")] public int Index { get; set; }
    }
}