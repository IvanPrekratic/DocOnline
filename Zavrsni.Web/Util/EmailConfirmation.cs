using System.Net.Mail;
using System.Net;
using MimeKit;
using MailKit.Net.Smtp;

namespace Zavrsni.Web.Util
{
    public class EmailConfirmation
    {
        /*
        public static IConfiguration _config;
        public EmailConfirmation(IConfiguration config)
        {
            _config = config;
        }
        */


        public async Task<Task> SendEmail(string Email, string Subject, string HtmlMessage)
        {
            var emailtosend = new MimeMessage();
            emailtosend.From.Add(MailboxAddress.Parse("doc.online.hr@gmail.com"));
            emailtosend.To.Add(MailboxAddress.Parse(Email));
            emailtosend.Subject = Subject;
            emailtosend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = HtmlMessage };
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate("doc.online.hr@gmail.com", "daphtcaudsgcttdt");
                smtp.Send(emailtosend);
                smtp.Disconnect(true);
            }
            return Task.CompletedTask;
        }
        /*
        public static bool SendEmailStari(string SenderEmail, string Subject, string Message, bool IsBodyHtml = false)
        {
            bool status = false;
            try
            {
                string HostAddress = "smtp.gmail.com";
                string FormEmailId = "doc.online.hr@gmail.com";
                string Password = "DocOnline21";
                string Port = "587";
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
        */
    }
}
