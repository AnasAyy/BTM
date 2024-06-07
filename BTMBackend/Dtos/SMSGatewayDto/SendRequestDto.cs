using System.Text.Json.Serialization;

namespace BTMBackend.Dtos.SMSGatewayDto
{
    public class SendRequestDto
    {
        [JsonPropertyName("src")]
        public string Source { get; set; } = null!;

        [JsonPropertyName("body")]
        public string Message { get; set; } = null!;

        [JsonPropertyName("dests")]
        public List<string> Destination { get; set; } = null!;
    }
}
