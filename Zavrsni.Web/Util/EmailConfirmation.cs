using System.Net.Mail;
using System.Net;
using MimeKit;
using MailKit.Net.Smtp;

namespace Zavrsni.Web.Util
{
    public class EmailConfirmation
    {
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
    }
}
