using mail_back.Api;
using mail_back.Converter;
using mail_back.Mail;
using mail_back.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        public QuoteJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap jobData = context.Trigger.JobDataMap;
            string filename = jobData.GetInt("idtask").ToString() + ".csv";
            string usermail = jobData.GetString("usermail");
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var mailSender = scope.ServiceProvider.GetService<IMailSender>();
                var quoteApi = scope.ServiceProvider.GetService<IQuote>();
                var csv = scope.ServiceProvider.GetService<ICSVConvert>();
                var c = quoteApi.GetData().Result;
                csv.Convert(c, filename);
                await mailSender.Send(filename, usermail);

                TaskRepository taskRepository = new TaskRepository(GetDBConnString());
                taskRepository.UpdateLastTime(jobData.GetInt("idtask"));

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
