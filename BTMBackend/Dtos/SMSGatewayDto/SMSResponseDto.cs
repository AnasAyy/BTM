using System.Text.Json.Serialization;

namespace BTMBackend.Dtos.SMSGatewayDto
{
    public class SMSResponseDto
    {
        [JsonPropertyName("message_id")]
        public int MessageId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = null!;

        [JsonPropertyName("remarks")]
        public string Remarks { get; set; } = null!;

        [JsonPropertyName("uid")]
        public string UID { get; set; } = null!;

        [JsonPropertyName("phonenumber")]
        public string PhoneNumber { get; set; } = null!;
    }
}