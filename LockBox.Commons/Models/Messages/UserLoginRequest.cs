using System.ComponentModel.DataAnnotations;

namespace LockBox.Commons.Models.Messages
{
    public class UserLoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6, ErrorMessage = "Your password needs to have at least 6 digits")]
        public string Password { get; set; } = string.Empty;
    }
}
