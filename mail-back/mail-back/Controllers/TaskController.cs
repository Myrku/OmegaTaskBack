using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using mail_back.Api;
using mail_back.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using mail_back.Mail;
using mail_back.Converter;
using mail_back.Models;
using mail_back.Jobs;

namespace mail_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        TaskRepository db;
        ApiRepository apidb;
        public TaskController(IConfiguration configuration, IServiceProvider service)
        {
            string connectionString = configuration.GetConnectionString("sqlite");
            db = new TaskRepository(connectionString);
            apidb = new ApiRepository(connectionString);
            //JobScheduler.Start(service);
            //CovidApi covidApi = new CovidApi();
            //var c = covidApi.GetData(string.Empty).Result;
            //var c = covidApi.GetData("BY").Result;

            //CSVConverter converter = new CSVConverter();
            //converter.Convert<Covid>(c, "covid.csv");

            //ForexPairApi stockApi = new ForexPairApi();
            //stockApi.GetData("USD/BYN");

            //QuoteApi quoteApi = new QuoteApi();
            //quoteApi.GetData();

            //MailSender mailSender = new MailSender(configuration);
            //mailSender.Send("covid.csv", "DenisV_1@mail.ru");
        }

        [HttpGet]
        [Route("get")]
        public IActionResult GetTasks()
        {
            return Ok(new
            {
                tasks = db.GetTasksByUserId(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
            });
        }

        [HttpPost]
        [Route("add")]
        public IActionResult AddTast([FromBody] Models.Task task)
        {
            if (task != null)
            {
                db.AddTask(task, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult DeleteTask(int id)
        {
            db.DeleteTaskById(id);
            return Ok();
        }

        [HttpGet]
        [Route("getapis")]
        public IActionResult GetApis()
        {
            return Ok(new
            {
                apis = apidb.GetApis()
            });
        }
    }
}
