using WebApp1.Core.Models;

namespace WebApp1.Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> GetUserByEmail(string email);
        Task<int> RegisterUser(User user);
        string GenerateJwtToken(User user);
    }
}
