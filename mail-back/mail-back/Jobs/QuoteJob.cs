using mail_back.Api;
using mail_back.Converter;
using mail_back.Mail;
using mail_back.Models;
using mail_back.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace mail_back.Jobs
{
    public class QuoteJob : IJob
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string conStr;


        public QuoteJob(IServiceScopeFactory serviceScopeFactory, IOptionsSnapshot<DBConfig> dbConfig)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            conStr = dbConfig.Value.SqliteConnection;
        }
        public async System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap jobData = context.Trigger.JobDataMap;
                string fileName = jobData.GetInt("idTask").ToString() + ".csv";
                string userMail = jobData.GetString("userMail");
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var mailSender = scope.ServiceProvider.GetService<IMailSender>();
                    var quoteApi = scope.ServiceProvider.GetService<IQuote>();
                    var csv = scope.ServiceProvider.GetService<ICSVConvert>();
                    var c = quoteApi.GetData().Result;
                    csv.Convert(c, fileName);
                    await mailSender.Send(fileName, userMail);

                    TaskRepository taskRepository = new TaskRepository(conStr);
                    taskRepository.UpdateLastTime(jobData.GetInt("idTask"));

                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}
