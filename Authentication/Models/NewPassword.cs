using System.ComponentModel.DataAnnotations;

namespace Authentication.Models
{
    public class NewPassword
    {
        [Required, StringLength(50, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
        [Compare("Password", ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
