using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace mail_back.Mail
{
    public class MailSender: IMailSender
    {
        string loginMail;
        string mailPassword;
        public MailSender(IConfiguration configuration)
        {
            loginMail = configuration.GetSection("MailSender").GetSection("login").Value;
            mailPassword = configuration.GetSection("MailSender").GetSection("password").Value;
        }

        // Принимает имя файла и отправляет этот файл в письме
        public async Task Send(string fileName, string userMail)
        {
            MailAddress from = new MailAddress(loginMail, "TaskMail");
            MailAddress to = new MailAddress(userMail);
            MailMessage mail = new MailMessage(from, to);
            mail.Subject = "Task";
            mail.Body = "<h1>Тест</h1>";
            mail.IsBodyHtml = true;
            mail.Attachments.Add(new Attachment(fileName));
            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.Credentials = new NetworkCredential(loginMail, mailPassword);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mail);
                smtp.Dispose();
            }
            mail.Dispose();
            DeleteFile(fileName);
        }

        public void DeleteFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }       
    }
}
