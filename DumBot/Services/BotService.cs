using System;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Linq;
using DumBot.Models;

namespace DumBot.Services
{
    public class BotService : IBotService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BotService> _logger;

        private readonly string _sendMessageUrl;
        private readonly string _getUsersUrl;
        private readonly string _searchDocsUrl;
        private readonly string _accessToken;
        private readonly string _apiVersion;

        public BotService(IConfiguration configuration, ILogger<BotService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _sendMessageUrl = _configuration.GetSection("AppSettings")["SendMessageUrl"];
            _getUsersUrl = _configuration.GetSection("AppSettings")["GetUsersUrl"];
            _searchDocsUrl = _configuration.GetSection("AppSettings")["SearchDocsUrl"];
            _accessToken = _configuration["AccessToken"];
            _apiVersion = _configuration.GetSection("AppSettings")["VkApiVersion"];
        }

        public async Task SendMessageAsync(int userId, string message, string attachment = "")
        {
            HttpClient httpClient = new HttpClient();
            
            var stringContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("user_id", userId.ToString()),
                new KeyValuePair<string, string>("message", message),
                new KeyValuePair<string, string>("access_token", _accessToken),
                new KeyValuePair<string, string>("v", _apiVersion),
                new KeyValuePair<string, string>("attachment", attachment)
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
                    var firstName = responseJson.FirstOrDefault().Value<string>("first_name") ?? string.Empty;

                    return string.IsNullOrEmpty(firstName)
                        ? responseJson.FirstOrDefault().Value<string>("last_name") ?? string.Empty
                        : firstName;
                }
            }
            else
            {
                _logger.LogError($"GetUsers for userId {userId} failed. Status code: {response.StatusCode}");
                return string.Empty;
            }
        }

        public async Task HandleMessageAsync(string message, int userId)
        {
            if (message.Trim().StartsWith('/'))
            {
                string command = message
                    .Trim()
                    .ToLowerInvariant()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()
                    .TrimStart('/');
                
                if (string.Compare(command, BotCommands.Help, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    await SendMessageAsync(userId,
                        $@"&#128220; /{BotCommands.Help} - список команд
&#127926; /{BotCommands.Music} - случайная аудиозапись
&#128008; /{BotCommands.CatGif} - случайный котик");
                    return;
                }

                if (string.Compare(command, BotCommands.Music, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    await SendMessageAsync(userId, "Функция в разработке...");
                    return;
                }

                if (string.Compare(command, BotCommands.CatGif, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    var attachmentString = await GetRandomDocAsync("cat gif");
                    if (string.IsNullOrEmpty(attachmentString))
                    {
                        await SendMessageAsync(userId, "Не удалось найти котиков &#128575;. Попробуйте позже");
                        return;
                    }
                    else
                    {
                        await SendMessageAsync(userId, string.Empty, attachmentString);
                        return;
                    }
                }
            }
            else if (message.ToLowerInvariant().Contains(BotCommands.Hi.ToLowerInvariant()))
            {
                string userName = await GetUserNameAsync(userId);
                string replyMessage = string.IsNullOrEmpty(userName) ? $"{BotCommands.Hi}!" : $"{BotCommands.Hi}, {userName}!";
                await SendMessageAsync(userId, replyMessage);
                return;                  
            }

            await SendMessageAsync(userId, $"Неизвестная команда. Наберите /{BotCommands.Help} для вывода списка команд");
        }

        public async Task<string> GetRandomDocAsync(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                throw new ArgumentException();
            }

            HttpClient httpClient = new HttpClient();

            Random random = new Random();
            int offset = random.Next(1, 1000);

            var stringContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("q", searchString),
                new KeyValuePair<string, string>("offset", offset.ToString()),
                new KeyValuePair<string, string>("count", "1"),
                new KeyValuePair<string, string>("access_token", _accessToken),
                new KeyValuePair<string, string>("v", _apiVersion)
            });

            var response = await httpClient.PostAsync(_searchDocsUrl, stringContent);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var jsonResult = JObject.Parse(JsonConvert.DeserializeObject(result).ToString());
                if (jsonResult.TryGetValue("error", StringComparison.OrdinalIgnoreCase, out JToken error))
                {
                    _logger.LogError($"Docs search failed. Error code: {error["error_code"]}, error message: {error["error_msg"]}");
                    return string.Empty;
                }
                else
                {
                    var responseJson = jsonResult["response"];
                    var docId = responseJson["items"].FirstOrDefault()?.Value<string>("id");
                    var ownerId = responseJson["items"].FirstOrDefault()?.Value<string>("owner_id");
                    
                    if (string.IsNullOrEmpty(docId) || string.IsNullOrEmpty(ownerId))
                    {
                        return string.Empty;
                    }

                    return $"doc{ownerId}_{docId}";
                }
            }
            else
            {
                _logger.LogError($"Docs search failed. Status code: {response.StatusCode}");
                return string.Empty;
            }
        }
    }
}
