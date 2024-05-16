namespace LockBox.Models.Messages
{
    public class UserEmailVerificationRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
