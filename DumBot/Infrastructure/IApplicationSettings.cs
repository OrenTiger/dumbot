namespace DumBot.Infrastructure
{
    public interface IApplicationSettings
    {
        string SendMessageUrl { get; }
        string GetUsersUrl { get; }
        string SearchDocsUrl { get; }
        string GetWeatherInfoUrl { get; }
        string AccessToken { get; }
        string ApiVersion { get; }
        string WeatherApiAccessToken { get; }
        string DocsSearchString { get; }
        int ForecastIntervalCount { get; }
        int ServerConfirmationGroupId { get; }
        string ServerConfirmationReplyString { get; }
    }
}
