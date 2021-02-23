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
    public class ForexJob : IJob
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string conStr;


        public ForexJob(IServiceScopeFactory serviceScopeFactory, IOptionsSnapshot<DBConfig> dbConfig)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            conStr = dbConfig.Value.SqliteConnection;
        }
        public async System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap jobData = context.Trigger.JobDataMap;
                string forexParam = jobData.GetString("apiParam");
                string fileName = jobData.GetInt("idTask").ToString() + ".csv";
                string userMail = jobData.GetString("userMail");
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var mailSender = scope.ServiceProvider.GetService<IMailSender>();
                    var forexApi = scope.ServiceProvider.GetService<IForex>();
                    var csv = scope.ServiceProvider.GetService<ICSVConvert>();
                    var c = forexApi.GetData(forexParam).Result;
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
