using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mail_back.Mail
{
    interface IMailSender
    {
        Task Send(string fileName, string userMail);
        void DeleteFile(string fileName);
    }
}
