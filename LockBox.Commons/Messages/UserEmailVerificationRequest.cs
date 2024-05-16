using System.ComponentModel.DataAnnotations;

namespace LockBox.Commons.Messages
{
    public class UserEmailVerificationRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
