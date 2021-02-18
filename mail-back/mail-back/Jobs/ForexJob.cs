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
    public class ForexJob : IJob
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public ForexJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap jobData = context.Trigger.JobDataMap;
            string forexparam = jobData.GetString("param");
            string filename = jobData.GetInt("idtask").ToString() + ".csv";
            string usermail = jobData.GetString("usermail");
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var mailSender = scope.ServiceProvider.GetService<IMailSender>();
                var forexApi = scope.ServiceProvider.GetService<IForex>();
                var csv = scope.ServiceProvider.GetService<ICSVConvert>();
                var c = forexApi.GetData(forexparam).Result;
                //csv.Convert(c, filename);
                //await mailSender.Send(filename, usermail);

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
