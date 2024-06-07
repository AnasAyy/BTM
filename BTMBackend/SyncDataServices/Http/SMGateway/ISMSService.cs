using Azure;
using BTMBackend.Dtos.SMSGatewayDto;
using BTMBackend.Utilities;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace BTMBackend.SyncDataServices.Http.SMGateway
{
    public interface ISMSService
    {
        Task<SMSResponseDto?> SendSMS(SendMessageRequestDto requestDto);
        Task<bool> SendMessage(SendRequestDto requestDto, string token);
    }
    public class SMSService : ISMSService
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public SMSService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "BTMSystem");
            _httpClient.BaseAddress = new Uri($"{_configuration["SMSService"]}");
        }
        public async Task<SMSResponseDto?> SendSMS(SendMessageRequestDto requestDto)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/SendSMS");
            requestMessage.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestDto), Encoding.UTF8, "application/json");
            using var res = await _httpClient.SendAsync(requestMessage);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                try
                {
                    SMSResponseDto? items = System.Text.Json.JsonSerializer.Deserialize<SMSResponseDto?>(result);
                    if (items != null)
                    {
                        return items;
                    }
                }
                catch { }
            }
            return null;

        }

        public async Task<bool> SendMessage(SendRequestDto requestDto , string token)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"/msgs/sms");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            requestMessage.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestDto), Encoding.UTF8, "application/json");
            using var res = await _httpClient.SendAsync(requestMessage);
            return res.IsSuccessStatusCode;
        }
    }
}
