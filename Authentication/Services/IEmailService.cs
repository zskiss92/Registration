using Authentication.Models;

namespace Authentication.Services
{
    public interface IEmailService
    {
        void SendEmail(EmailDto request);
    }
}
