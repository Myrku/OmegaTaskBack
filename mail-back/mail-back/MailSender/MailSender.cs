using mail_back.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NLog;
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
        Logger logger = LogManager.GetCurrentClassLogger();

        private MailSenderConfig mailConfug;
        private const string displayMailName = "TaskMail";
        private const string subjectMail = "Данные от API";
        private string messageBody = $"<h1>Данные на момент времени {DateTime.Now}</h1>";

        public MailSender(IOptionsSnapshot<MailSenderConfig> mailConfug)
        {
            this.mailConfug = mailConfug.Value;
        }

        // Принимает имя файла и отправляет этот файл в письме
        public async System.Threading.Tasks.Task Send(string fileName, string userMail)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(fileName) && !string.IsNullOrWhiteSpace(userMail))
                {
                    MailAddress from = new MailAddress(mailConfug.Login, displayMailName);
                    MailAddress to = new MailAddress(userMail);
                    MailMessage mail = new MailMessage(from, to);
                    mail.Subject = subjectMail;
                    mail.Body = messageBody;
                    mail.IsBodyHtml = true;
                    mail.Attachments.Add(new Attachment(fileName));
                    using (SmtpClient smtp = new SmtpClient(mailConfug.SmtpName, mailConfug.Port))
                    {
                        smtp.Credentials = new NetworkCredential(mailConfug.Login, mailConfug.Password);
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(mail);
                        smtp.Dispose();
                    }
                    mail.Dispose();
                    DeleteFile(fileName);
                }
                else
                {
                    throw new ArgumentException("fileName, userMail");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            
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
