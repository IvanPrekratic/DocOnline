using System.Net.Mail;
using System.Net;

namespace Zavrsni.Web.Util
{
    public class EmailConfirmation
    {
        public static IConfiguration _config;
        public EmailConfirmation(IConfiguration config)
        {
            _config = config;
        }
        public static bool SendEmail(string SenderEmail, string Subject, string Message, bool IsBodyHtml = false)
        {
            bool status = false;
            try
            {
                string HostAddress = _config.GetValue<string>("MailSettings:Host");
                string FormEmailId = _config.GetValue<string>("MailSettings:MailFrom");
                string Password = _config.GetValue<string>("MailSettings:Password");
                string Port = _config.GetValue<string>("MailSettings:Port");
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(FormEmailId);
                mailMessage.Subject = Subject;
                mailMessage.Body = Message;
                mailMessage.IsBodyHtml = IsBodyHtml;
                mailMessage.To.Add(new MailAddress(SenderEmail));
                SmtpClient smtp = new SmtpClient();
                smtp.Host = HostAddress;
                smtp.EnableSsl = true;
                NetworkCredential networkCredential = new NetworkCredential();
                networkCredential.UserName = mailMessage.From.Address;
                networkCredential.Password = Password;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = networkCredential;
                smtp.Port = Convert.ToInt32(Port);
                smtp.Send(mailMessage);
                status = true;
                return status;
            }
            catch (Exception e)
            {
                return status;
            }
        }
    }
}
