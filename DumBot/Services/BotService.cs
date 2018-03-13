using System;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Linq;

namespace DumBot.Services
{
    public class BotService : IBotService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BotService> _logger;

        private readonly string _sendMessageUrl;
        private readonly string _getUsersUrl;
        private readonly string _accessToken;
        private readonly string _apiVersion;

        public BotService(IConfiguration configuration, ILogger<BotService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _sendMessageUrl = _configuration.GetSection("AppSettings")["SendMessageUrl"];
            _getUsersUrl = _configuration.GetSection("AppSettings")["GetUsersUrl"];
            _accessToken = _configuration["AccessToken"];
            _apiVersion = _configuration.GetSection("AppSettings")["VkApiVersion"];
        }

        public async void SendMessageAsync(int userId, string message)
        {
            HttpClient httpClient = new HttpClient();
            
            var stringContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("user_id", userId.ToString()),
                new KeyValuePair<string, string>("message", message),
                new KeyValuePair<string, string>("access_token", _accessToken),
                new KeyValuePair<string, string>("v", _apiVersion)
            });

            var response = await httpClient.PostAsync(_sendMessageUrl, stringContent);

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

        public async Task<string> GetUserNameAsync(int userId)
        {
            HttpClient httpClient = new HttpClient();

            var stringContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("user_id", userId.ToString()),
                new KeyValuePair<string, string>("access_token", _accessToken),
                new KeyValuePair<string, string>("v", _apiVersion)
            });

            var response = await httpClient.PostAsync(_getUsersUrl, stringContent);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var jsonResult = JObject.Parse(JsonConvert.DeserializeObject(result).ToString());
                if (jsonResult.TryGetValue("error", StringComparison.OrdinalIgnoreCase, out JToken error))
                {
                    _logger.LogError($"GetUsers for userId {userId} failed. Error code: {error["error_code"]}, error message: {error["error_msg"]}");
                    return string.Empty;
                }
                else
                {
                    var responseJson = jsonResult["response"];
                    var firstName = responseJson.FirstOrDefault().Value<string>("first_name");

                    return string.IsNullOrEmpty(firstName)
                        ? responseJson.FirstOrDefault().Value<string>("last_name")
                        : firstName;
                }
            }
            else
            {
                _logger.LogError($"GetUsers for userId {userId} failed. Status code: {response.StatusCode}");
                return string.Empty;
            }
        }
    }
}
