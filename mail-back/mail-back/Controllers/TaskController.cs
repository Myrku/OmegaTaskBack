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
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using NLog;

namespace mail_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        TaskRepository db;
        ApiRepository apiDb;
        UserRepository userDb;
        Logger logger = LogManager.GetCurrentClassLogger();

        public TaskController(IOptionsSnapshot<DBConfig> dbConfig)
        {
            string connectionString = dbConfig.Value.SqliteConnection;
            db = new TaskRepository(connectionString);
            apiDb = new ApiRepository(connectionString);
            userDb = new UserRepository(connectionString);
        }

        [HttpGet]
        [Route("stat")]
        [Authorize(Roles = "1")]
        public IActionResult GetStat() // статистика для админа (имя пользователя, кол-во задач и общее кол-во выполнений)
        {
            try
            {
                return Ok(new
                {
                    stat = from tasks in db.GetTasks()
                           join users in userDb.GetUsers() on tasks.UserId equals users.Id
                           group tasks by users.Username into x
                           select new
                           {
                               username = x.Key,
                               countTasks = x.Count(),
                               countExecute = x.Sum(x => x.Count)
                           }
                });
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }
        }

        [HttpGet]
        [Route("get")]
        public IActionResult GetTasks()
        {
            try
            {
                return Ok(new
                {
                    tasks = db.GetTasksByUserId(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                });
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("add")]
        public IActionResult AddTask([FromBody] Models.Task task)
        {
            try
            {
                if (task != null)
                {
                    var user = userDb.GetUserById(Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
                    task.Id = db.AddTaskWithGetRowId(task, user.Id.ToString()).Result;
                    JobScheduler.AddTaskTriggerForJob(task, user);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }
        }

        [HttpPut]
        [Route("update")]
        public IActionResult Update([FromBody] Models.Task task)
        {
            try
            {
                if (task != null)
                {
                    var user = userDb.GetUserById(Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
                    db.UpdateTask(task);
                    JobScheduler.UpdateTaskTrigger(task, user);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                return Ok("Error update task");
            }
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult DeleteTask(int id)
        {
            try
            {
                db.DeleteTaskById(id);
                return Ok();
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }
        }

        [HttpGet]
        [Route("getapis")]
        public IActionResult GetApis()
        {
            try
            {
                return Ok(new
                {
                    apis = apiDb.GetApis()
                });
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }
        }
    }
}
