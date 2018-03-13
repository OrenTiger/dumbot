using System.Threading.Tasks;

namespace DumBot.Services
{
    public interface IBotService
    {
        void SendMessageAsync(int userId, string text);
        Task<string> GetUserNameAsync(int userId);
    }
}
