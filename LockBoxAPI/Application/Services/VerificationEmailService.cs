using System.Net.Mail;
using System.Net;

namespace LockBoxAPI.Application.Services
{
    public class VerificationEmailService
    {
        public string VerificationEmail(string userEmail)
        {
            try
            {
                string verificationCode = GenerateRandomDigits(6);
                MailMessage mail = WriteEmail(userEmail, verificationCode);
                SmtpClient smtp = PrepareSending();
                smtp.Send(mail);
                return verificationCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
        public static MailMessage WriteEmail(string userEmail, string verificationCode)
        {
            MailMessage message = new MailMessage();
            message.To.Add(userEmail);
            message.From = new MailAddress("quorumChat@gmail.com");
            message.Body = $"Your verification code is: {verificationCode}\n";
            message.Subject = "Verification Code";

            return message;
        }
        private static SmtpClient PrepareSending()
        {
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.EnableSsl = true;
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential("quorumChat@gmail.com", "pjkl dmbr ijzx ggkm");
            return smtp;
        }
        public static string GenerateRandomDigits(int numberOfDigits)
        {
            string verificationCode = "";
            Random random = new Random();
            for (int i = 0; i < numberOfDigits; i++)
            {
                int randomNumber = random.Next(10);
                verificationCode += randomNumber.ToString();
            }
            return verificationCode;
        }
    }
}