using System;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DumBot.Services
{
    public class BotService : IBotService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BotService> _logger;

        public BotService(IConfiguration configuration, ILogger<BotService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async void SendMessage(int userId, string message)
        {
            string sendMessageUrl = _configuration.GetSection("AppSettings")["SendMessageUrl"];
            string accessToken = _configuration["AccessToken"];
            string apiVersion = _configuration.GetSection("AppSettings")["VkApiVersion"];

            HttpClient httpClient = new HttpClient();
            
            var stringContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("user_id", userId.ToString()),
                new KeyValuePair<string, string>("message", message),
                new KeyValuePair<string, string>("access_token", accessToken),
                new KeyValuePair<string, string>("v", apiVersion)
            });

            var response = await httpClient.PostAsync(sendMessageUrl, stringContent);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var jsonResult = JObject.Parse(JsonConvert.DeserializeObject(result).ToString());  
                if (jsonResult.TryGetValue("error", StringComparison.OrdinalIgnoreCase, out JToken error))
                {
                    _logger.LogError($"Send message for userId {userId} failed. Error code: {error["error_code"]}, error message: {error["error_msg"]}");
                }
            }
            else
            {
                _logger.LogError($"Send message for userId={userId} failed. Status code: {response.StatusCode}");
            }
        }
    }
}
