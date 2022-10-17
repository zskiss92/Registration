using Authentication.Models;

namespace Authentication.Services
{
    public interface IAuthService
    {
        Task<bool> Register(User user, string password);
        Task<bool> UserExists(string email);
        Task<string> Login(string email, string password);
        string GetUserId();
        Task<bool> SendVerificationCode(User user);
        Task<string> VerifyEmail(string id);
    }
}
