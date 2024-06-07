using System.Text.Json.Serialization;

namespace BTMBackend.Dtos.SMSGatewayDto
{
    public class SendMessageRequestDto
    {
        [JsonPropertyName("api_id")]
        public string ApiId { get; set; } = "API49938079118";

        [JsonPropertyName("api_password")]
        public string ApiPassword { get; set; } = "Gateway@123";

        [JsonPropertyName("sms_type")]
        public string SMSType { get; set; } = "T";

        [JsonPropertyName("encoding")]
        public string Encoding { get; set; } = "T";

        [JsonPropertyName("sender_id")]
        public string SenderId { get; set; } = "BRONZE NET";

        [JsonPropertyName("phonenumber")]
        public string? PhoneNumber { get; set; } = null!;

        [JsonPropertyName("templateid")]
        public string TemplateId { get; set; } = null!;

        [JsonPropertyName("textmessage")]
        public string TextMessage { get; set; } = null!;

        [JsonPropertyName("V1")]
        public string? V1 { get; set; }

        [JsonPropertyName("V2")]
        public string? V2 { get; set; }

        [JsonPropertyName("V3")]
        public string? V3 { get; set; }

        [JsonPropertyName("V4")]
        public string? V4 { get; set; }

        [JsonPropertyName("V5")]
        public string? V5 { get; set; }

        [JsonPropertyName("ValidityPeriodInSeconds")]
        public string ValidityPeriodInSeconds { get; set; } = "60";

        [JsonPropertyName("uid")]
        public string UID { get; set; } = "BRONZE NET";

        [JsonPropertyName("callback_url")]
        public string CallBackUrl { get; set; } = "https://brnznetfilter.com/";

        [JsonPropertyName("pe_id")]
        public string PeId { get; set; } = "BRONZE";

        [JsonPropertyName("template_id")]
        public string TemplateIdName { get; set; } = null!;
    }
}
