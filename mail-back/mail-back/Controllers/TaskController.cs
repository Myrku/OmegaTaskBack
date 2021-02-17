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

namespace mail_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        TaskRepository db;
        ApiRepository apidb;
        public TaskController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("sqlite");
            db = new TaskRepository(connectionString);
            apidb = new ApiRepository(connectionString);
            //CovidApi covidApi = new CovidApi();
            //covidApi.GetData(string.Empty);
            //covidApi.GetData("BY");

            ForexPairApi stockApi = new ForexPairApi();
            stockApi.GetData("USD/BYN");

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
