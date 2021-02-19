using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using mail_back.Repository;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.IO;
using mail_back.Models;
using System.Collections.Specialized;

namespace mail_back.Jobs
{
    public class JobScheduler
    {
        private enum APIID
        {
            COVID = 1, //(1 - id covid api в БД)
            QUOTE = 2, //(2 - id forex api в БД)
            FOREX = 3//(3 - id quote api в БД)
        }
        static IScheduler scheduler;
        public static async void Start(IServiceProvider serviceProvider)
        {

            StdSchedulerFactory factory = new StdSchedulerFactory(GetThreadConstraint());
            scheduler = factory.GetScheduler().Result;
            scheduler.JobFactory = serviceProvider.GetService<JobFactory>();
            await scheduler.Start();

            IJobDetail covidJob = JobBuilder.Create<CovidJob>()
                .StoreDurably()
                .WithIdentity("CovidJob")
                .Build();
            await scheduler.AddJob(covidJob, true);
            IJobDetail forexJob = JobBuilder.Create<ForexJob>()
                .StoreDurably()
                .WithIdentity("ForexJob")
                .Build();
            await scheduler.AddJob(forexJob, true);
            IJobDetail quoteJob = JobBuilder.Create<QuoteJob>()
                .StoreDurably()
                .WithIdentity("QuoteJob")
                .Build();
            await scheduler.AddJob(quoteJob, true);

            var dbConnection = GetDBConnString();
            TaskRepository taskRepository = new TaskRepository(dbConnection);
            var tasks = taskRepository.GetTasks();
            UserRepository userRepository = new UserRepository(dbConnection);


            foreach (var task in tasks)
            {
                var user = userRepository.GetUserById(task.userid);
                AddTaskTriggerForJob(task, user);
            }
        }


        public static async void AddTaskTriggerForJob(Models.Task task, User user)
        {
            TriggerBuilder triggerBuilder = TriggerBuilder.Create()
                        .WithIdentity(task.id.ToString())
                        .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(task.period)
                        .RepeatForever());
            if (DateTime.Now >= DateTime.Parse(task.starttime))
            {
                triggerBuilder.StartNow();
            }
            else
            {
                triggerBuilder.StartAt(DateTimeOffset.Parse(task.starttime));
            }

            ITrigger trigger = null;
            if (task.apiid == (int)APIID.COVID) // создание триггеров на задачи по получанию данных covid 
            {
                trigger = triggerBuilder.ForJob("CovidJob").Build();

            }
            else if (task.apiid == (int)APIID.FOREX) // создание триггеров на задачи по получанию данных forex 
            {
                trigger = triggerBuilder.ForJob("ForexJob").Build();

            }
            else if (task.apiid == (int)APIID.QUOTE) // создание триггеров на задачи по получанию данных quote 
            {
                trigger = triggerBuilder.ForJob("QuoteJob").Build();
            }
            trigger.JobDataMap["param"] = task.apiparam;
            trigger.JobDataMap["idtask"] = task.id;
            trigger.JobDataMap["usermail"] = user.email;
            await scheduler.ScheduleJob(trigger);
        }
        public static async void UpdateTaskTrigger(Models.Task task, User user)
        {
            await scheduler.UnscheduleJob(new TriggerKey(task.id.ToString()));
            AddTaskTriggerForJob(task, user);
        }
        private static NameValueCollection GetThreadConstraint() // получаем ограничение на кол-во одновременных потоков для задач
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(ApplicationExeDirectory())
            .AddJsonFile("appsettings.json").Build();
            return new NameValueCollection { { builder["ThreadConstraint:param"], builder["ThreadConstraint:countThreads"] } };
        }
        private static string GetDBConnString()
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
