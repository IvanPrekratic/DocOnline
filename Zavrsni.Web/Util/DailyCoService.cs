using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Zavrsni.Web.Util
{
    public class DailyCoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public DailyCoService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<string> CreateRoomAsync(string roomName, DateTime startTime)
        {
            var requestUrl = "https://api.daily.co/v1/rooms";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var requestData = new
            {
                name = roomName,
                properties = new
                {
                    start_audio_off = true,
                    start_video_off = true,
                    enable_chat = true,
                    nbf = new DateTimeOffset(startTime.AddMinutes(-15)).ToUnixTimeSeconds(),
                    exp = new DateTimeOffset(startTime.AddHours(1.5)).ToUnixTimeSeconds(),

                }
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(requestData), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUrl, requestContent);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var roomInfo = JsonSerializer.Deserialize<RoomResponse>(jsonResponse);
                return roomInfo.url;
            }
            else
            {
                throw new Exception("Error creating room: " + response.ReasonPhrase);
            }
        }
    }

    public class RoomResponse
    {
        public string url { get; set; }
    }
}
