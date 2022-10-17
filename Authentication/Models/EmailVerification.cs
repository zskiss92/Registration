using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication.Models
{
    public class EmailVerification
    {
        [Key]
        public Guid Id { get; set; }
        public Guid EmailId { get; set; }
        [ForeignKey("EmailId")]
        public User? Email { get; set; }
        public string VerificationKey { get; set; } = string.Empty;
        public DateTime KeyCreated { get; set; } = DateTime.Now;
        public DateTime VerificationTime { get; set; }
    }
}
