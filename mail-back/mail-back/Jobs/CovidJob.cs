using mail_back.Api;
using mail_back.Converter;
using mail_back.Mail;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;

namespace mail_back.Jobs
{
    public class CovidJob : IJob
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public CovidJob(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap jobData = context.Trigger.JobDataMap;
            string covidparam = jobData.GetString("covidparam");
            string filename = jobData.GetInt("idtask").ToString() + ".csv";
            string usermail = jobData.GetString("usermail");
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var mailSender = scope.ServiceProvider.GetService<IMailSender>();
                var covidApi = scope.ServiceProvider.GetService<ICovid>();
                var csv = scope.ServiceProvider.GetService<ICSVConvert>();
                var c = covidApi.GetData(covidparam).Result;
                //csv.Convert(c, filename);
                //await mailSender.Send(filename, usermail);

            }
        }
    }
}
