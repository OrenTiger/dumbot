using System.Threading.Tasks;

namespace DumBot.Services
{
    public interface IBotService
    {
        Task SendMessageAsync(int userId, string text, string attachment);
        Task<string> GetUserNameAsync(int userId);
        Task HandleMessageAsync(string message, int userId);
        Task<string> GetRandomDocAsync(string searchString);
    }
}
