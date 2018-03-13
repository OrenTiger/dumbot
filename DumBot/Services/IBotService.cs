namespace DumBot.Services
{
    public interface IBotService
    {
        void SendMessage(int userId, string text);
    }
}
