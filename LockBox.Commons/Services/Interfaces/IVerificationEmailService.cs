namespace LockBox.Commons.Services.Interfaces
{
    public interface IVerificationEmailService
    {
        string VerificationEmail(string userEmail);
    }
}
