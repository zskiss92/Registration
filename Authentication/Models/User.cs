using System.ComponentModel.DataAnnotations;

namespace Authentication.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public bool VerifiedEmail { get; set; } = false;

    }
}
