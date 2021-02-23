using mail_back.Api;
using mail_back.Converter;
using mail_back.Mail;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Microsoft.Extensions.Configuration;
using System.IO;
using mail_back.Repository;
using NLog;
using Microsoft.Extensions.Options;
using mail_back.Models;

namespace mail_back.Jobs
{
    public class CovidJob : IJob
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string conStr;

        public CovidJob(IServiceScopeFactory serviceScopeFactory, IOptionsSnapshot<DBConfig> dbConfig)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            conStr = dbConfig.Value.SqliteConnection;

        }
        public async System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap jobData = context.Trigger.JobDataMap;
                string covidParam = jobData.GetString("apiParam");
                string fileName = jobData.GetInt("idTask").ToString() + ".csv";
                string userMail = jobData.GetString("userMail");
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var mailSender = scope.ServiceProvider.GetService<IMailSender>();
                    var covidApi = scope.ServiceProvider.GetService<ICovid>();
                    var csv = scope.ServiceProvider.GetService<ICSVConvert>();
                    var c = covidApi.GetData(covidParam).Result;
                    csv.Convert(c, fileName);
                    await mailSender.Send(fileName, userMail);
                    TaskRepository taskRepository = new TaskRepository(conStr);
                    taskRepository.UpdateLastTime(jobData.GetInt("idTask"));
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}
