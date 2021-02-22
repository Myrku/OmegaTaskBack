using mail_back.Api;
using mail_back.Converter;
using mail_back.Mail;
using mail_back.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        public ForexJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap jobData = context.Trigger.JobDataMap;
                string forexParam = jobData.GetString("param");
                string fileName = jobData.GetInt("idtask").ToString() + ".csv";
                string userMail = jobData.GetString("usermail");
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var mailSender = scope.ServiceProvider.GetService<IMailSender>();
                    var forexApi = scope.ServiceProvider.GetService<IForex>();
                    var csv = scope.ServiceProvider.GetService<ICSVConvert>();
                    var c = forexApi.GetData(forexParam).Result;
                    csv.Convert(c, fileName);
                    await mailSender.Send(fileName, userMail);

                    TaskRepository taskRepository = new TaskRepository(GetDBConnString());
                    taskRepository.UpdateLastTime(jobData.GetInt("idtask"));

                }
            }
            catch(Exception ex)
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
