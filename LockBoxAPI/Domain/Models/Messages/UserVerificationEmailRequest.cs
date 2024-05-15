namespace LockBox.Models.Messages
{
    public class UserVerificationEmailRequest
    {
        public string Token { get; set; }
        public string Code { get; set; }
    }
}
