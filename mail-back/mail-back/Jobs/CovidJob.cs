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

namespace mail_back.Jobs
{
    public class CovidJob : IJob
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        Logger logger = LogManager.GetCurrentClassLogger();

        public CovidJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap jobData = context.Trigger.JobDataMap;
                string covidParam = jobData.GetString("param");
                string fileName = jobData.GetInt("idtask").ToString() + ".csv";
                string userMail = jobData.GetString("usermail");
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var mailSender = scope.ServiceProvider.GetService<IMailSender>();
                    var covidApi = scope.ServiceProvider.GetService<ICovid>();
                    var csv = scope.ServiceProvider.GetService<ICSVConvert>();
                    var c = covidApi.GetData(covidParam).Result;
                    csv.Convert(c, fileName);
                    await mailSender.Send(fileName, userMail);
                    TaskRepository taskRepository = new TaskRepository(GetDBConnString());
                    taskRepository.UpdateLastTime(jobData.GetInt("idtask"));
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
        private string GetDBConnString()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(ApplicationExeDirectory())
            .AddJsonFile("appsettings.json").Build();
            return builder["ConnectionStrings:sqlite"];
        }
        private static string ApplicationExeDirectory()
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var appRoot = Path.GetDirectoryName(location);

            return appRoot;
        }
    }
}
