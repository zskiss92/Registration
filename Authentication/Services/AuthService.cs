using Authentication.Data;
using Authentication.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Authentication.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        public AuthService(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _emailService = emailService;
        }

        public string GetUserId()
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<string> Login(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));

            if (user == null)
            {
                return "User not found";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return "Wrong password";
            }
            else if (!user.VerifiedEmail)
            {
                return "Email not verified";
            }
            else
            {
                return "Ok";
            }

        }

        public async Task<bool> Register(User user, string password)
        {
            if(await UserExists(user.Email))
            {
                return false;
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await SendVerificationCode(user);

            return true;
        }

        public async Task<bool> UserExists(string email)
        {
            if (await _context.Users.AnyAsync(
                user => user.Email.ToLower().Equals(email.ToLower())))
            {
                return true;
            }

            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash =
                    hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public async Task<bool> SendVerificationCode(User user)
        {
            var emailRequest = new EmailVerification();
            var emailDto = new EmailDto();

            string verificationCode = Guid.NewGuid().ToString("N");

            emailRequest.EmailId = user.Id;
            emailRequest.VerificationKey = verificationCode;

            emailDto.To = user.Email;
            emailDto.Subject = "Verify email address";
            emailDto.Body = "<h1>Please, click on the link below to verify your email address</h1></ br>"
                + "<a href='https://localhost:44359/api/Authentication/" + verificationCode
                + "'>https://localhost:44359/api/Authentication/" + verificationCode +"</a>";

            _emailService.SendEmail(emailDto);

            _context.EmailVerifications.Add(emailRequest);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string> VerifyEmail(string key)
        {
            var email = await _context.EmailVerifications.FirstOrDefaultAsync(e => e.VerificationKey == key);
            
            if(email == null)
            {
                return "Verification key not found";
            }

            email.VerificationTime = DateTime.Now;
            await _context.SaveChangesAsync();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == email.EmailId);

            if(user == null)
            {
                return "User not found";
            }

            user.VerifiedEmail = true;
            await _context.SaveChangesAsync();
            
            return "Ok";
        }
    }
}
