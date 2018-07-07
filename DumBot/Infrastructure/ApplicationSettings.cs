using Microsoft.Extensions.Configuration;

namespace DumBot.Infrastructure
{
    public class ApplicationSettings : IApplicationSettings
    {
        private readonly IConfiguration _configuration;

        public string SendMessageUrl => _configuration.GetSection("AppSettings")["SendMessageUrl"];

        public string GetUsersUrl => _configuration.GetSection("AppSettings")["GetUsersUrl"];

        public string SearchDocsUrl => _configuration.GetSection("AppSettings")["SearchDocsUrl"];

        public string GetWeatherInfoUrl => _configuration.GetSection("AppSettings")["GetWeatherInfoUrl"];

        public string AccessToken => _configuration.GetSection("AppSettings")["AccessToken"];

        public string ApiVersion => _configuration.GetSection("AppSettings")["VkApiVersion"];

        public string WeatherApiAccessToken => _configuration.GetSection("AppSettings")["WeatherApiAccessToken"];

        public string DocsSearchString => _configuration.GetSection("AppSettings")["DocsSearchString"];

        public int ForecastIntervalCount => int.Parse(_configuration.GetSection("AppSettings")["ForecastIntervalCount"]);

        public int ServerConfirmationGroupId => int.Parse(_configuration.GetSection("AppSettings")["ServerConfirmationGroupId"]);

        public string ServerConfirmationReplyString => _configuration.GetSection("AppSettings")["ServerConfirmationReplyString"];

        public ApplicationSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
