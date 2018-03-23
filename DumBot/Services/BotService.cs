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
using DumBot.Models.Forecast;
using DumBot.Models.Doc;
using System.Net;
using System.Text;
using DumBot.Resources;

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
        private readonly string _weatherApiAccessToken;
        private readonly string _getWeatherInfoUrl;
        private readonly string _apiVersion;
        private readonly string _docsSearchString;
        private readonly int _forecastIntervalCount;

        public BotService(IConfiguration configuration, ILogger<BotService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _sendMessageUrl = _configuration.GetSection("AppSettings")["SendMessageUrl"];
            _getUsersUrl = _configuration.GetSection("AppSettings")["GetUsersUrl"];
            _searchDocsUrl = _configuration.GetSection("AppSettings")["SearchDocsUrl"];
            _getWeatherInfoUrl = _configuration.GetSection("AppSettings")["GetWeatherInfoUrl"];
            _accessToken = _configuration["AccessToken"];
            _apiVersion = _configuration.GetSection("AppSettings")["VkApiVersion"];
            _weatherApiAccessToken = _configuration["WeatherApiAccessToken"];
            _docsSearchString = _configuration.GetSection("AppSettings")["DocsSearchString"];
            _forecastIntervalCount = int.Parse(_configuration.GetSection("AppSettings")["ForecastIntervalCount"]);
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
                    var replyMessage = new StringBuilder()
                        .AppendLine(string.Format(BotMessages.HelpCommandDescription, BotCommands.Help))
                        .AppendLine(string.Format(BotMessages.CatGifCommandDescription, BotCommands.CatGif))
                        .AppendLine(string.Format(BotMessages.WeatherCommandDescription, BotCommands.Weather))
                        .ToString();

                    await SendMessageAsync(userId, replyMessage);
                    return;
                }

                if (string.Compare(command, BotCommands.CatGif, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    var attachmentString = await GetRandomDocAsync(_docsSearchString);
                    
                    if (string.IsNullOrEmpty(attachmentString))
                    {
                        await SendMessageAsync(userId, BotMessages.CatsNotFound);
                        return;
                    }
                    else
                    {
                        await SendMessageAsync(userId, string.Empty, attachmentString);
                        return;
                    }
                }

                if (string.Compare(command, BotCommands.Weather, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    var city = message
                        .Replace(BotCommands.Weather, string.Empty, StringComparison.InvariantCultureIgnoreCase)
                        .Trim('/', ' ');

                    string weatherInfoMessage = string.Empty;

                    weatherInfoMessage = string.IsNullOrEmpty(city)
                        ? BotMessages.CityNotSpecified
                        : await GetWeatherInfoAsync(city);

                    await SendMessageAsync(userId, weatherInfoMessage);
                    return;
                }

                await SendMessageAsync(userId, $"{BotMessages.UnknownCommand}. {string.Format(BotMessages.UseHelp, BotCommands.Help)}");
                return;
            }
            else if (message.ToLowerInvariant().Contains(BotCommands.Hi))
            {
                string userName = await GetUserNameAsync(userId);
                string replyMessage = string.IsNullOrEmpty(userName) ? $"{BotMessages.Greeting}!" : $"{BotMessages.Greeting}, {userName}!";
                await SendMessageAsync(userId, replyMessage);
                return;                  
            }

            await SendMessageAsync(userId, $"{BotMessages.DumbBot}. {string.Format(BotMessages.UseHelp, BotCommands.Help)}");
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
                    try
                    {
                        var docs = JsonConvert.DeserializeObject<Docs>(jsonResult["response"].ToString());

                        if (docs.Items.Count == 0)
                            return string.Empty;

                        return $"doc{docs.Items.FirstOrDefault().Owner_id}_{docs.Items.FirstOrDefault().Id}";
                    }
                    catch (JsonSerializationException exception)
                    {
                        _logger.LogError($"Docs search failed. Exception while deserializing object. Exception details: {exception}");
                        return string.Empty;
                    }
                }
            }
            else
            {
                _logger.LogError($"Docs search failed. Status code: {response.StatusCode}");
                return string.Empty;
            }
        }

        public async Task<string> GetWeatherInfoAsync(string city)
        {
            if (string.IsNullOrEmpty(city)) 
            {
                throw new ArgumentException();
            }

            HttpResponseMessage response = new HttpResponseMessage();
            HttpClient httpClient = new HttpClient();

            try
            {
                response = await httpClient.GetAsync($"{_getWeatherInfoUrl}?APPID={_weatherApiAccessToken}&q={city}&units=metric");
            }
            catch (Exception exception)
            {
                _logger.LogError($"Get weather info failed. Exception details: {exception}");
                return BotMessages.TryAgainLater;
            }

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                try
                {
                    var forecast = JsonConvert.DeserializeObject<ForecastModel>(result);

                    StringBuilder resultString = new StringBuilder();

                    for (int i = 0; i <= _forecastIntervalCount; i++)
                    {
                        resultString
                            .AppendLine(string.Format(BotMessages.Forecast_Time, forecast.List[i].Dt_txt))
                            .AppendLine(string.Format(BotMessages.Forecast_Weather, forecast.List[i].Weather.FirstOrDefault()?.Description))
                            .AppendLine(string.Format(BotMessages.Forecast_Temperature, forecast.List[i].Main.Temp))
                            .AppendLine(string.Format(BotMessages.Forecast_Wind, forecast.List[i].Wind.Speed))
                            .AppendLine();
                    }

                    return resultString.ToString();
                }
                catch (JsonSerializationException exception)
                {
                    _logger.LogError($"Get weather info failed. Exception while deserializing object. Exception details: {exception}");
                    return BotMessages.TryAgainLater;
                }
                catch (Exception exception)
                {
                    _logger.LogError($"Get weather info failed. Exception details: {exception}");
                    return BotMessages.TryAgainLater;
                }
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return BotMessages.CityNotFound;

                _logger.LogError($"Get weather info failed. Status code: {response.StatusCode}");
                return BotMessages.TryAgainLater;
            }
        }
    }
}
