using mail_back.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace mail_back.Repository
{
    public class TaskRepository
    {
        private Repository<Models.Task> repository;
        const string TableName = "Tasks";
        public TaskRepository(string connectionString)
        {
            repository = new Repository<Models.Task>(connectionString);
        }

        public List<Models.Task> GetTasks()
        {
            SQLiteCommand command = new SQLiteCommand
            {
                CommandText = $"Select [Id], [UserId], [TaskName], [Description], [StartTime], [Period], [ApiId], [ApiParam], [LastStart], [Count] from {TableName}"
            };
            return repository.Get(command).Result.ToList();
        }
        public List<Models.Task> GetTasksByUserId(string userId)
        {
            SQLiteCommand command = new SQLiteCommand
            {
                CommandText = $"Select [Id], [UserId], [TaskName], [Description], [StartTime], [Period], [ApiId], [ApiParam], [LastStart], [Count] from {TableName}"
            };
            return repository.Get(command).Result.Where(x => x.UserId == Convert.ToInt32(userId)).ToList();
        }
        public async void AddTask(Models.Task task, string userId)
        {
            SQLiteCommand command = new SQLiteCommand($"Insert into {TableName}(userid, taskname, description, starttime, period, apiid, apiparam, laststart) " +
                $"Values(@userid, @taskname, @description, @starttime, @period, @apiid, @apiparam, @laststart);");
            command.Parameters.AddWithValue("@userid", Convert.ToInt32(userId));
            command.Parameters.AddWithValue("@taskname",task.TaskName);
            command.Parameters.AddWithValue("@description", task.Description);
            command.Parameters.AddWithValue("@starttime", task.StartTime);
            command.Parameters.AddWithValue("@period", task.Period);
            command.Parameters.AddWithValue("@apiid", task.ApiId);
            command.Parameters.AddWithValue("@apiparam", task.ApiParam);
            command.Parameters.AddWithValue("@laststart", task.LastStart);
            await repository.Add(command);

        }
        public async Task<int> AddTaskWithGetRowId(Models.Task task, string userId)
        {
            SQLiteCommand command = new SQLiteCommand($"Insert into {TableName}(userid, taskname, description, starttime, period, apiid, apiparam, laststart) " +
                $"Values(@userid, @taskname, @description, @starttime, @period, @apiid, @apiparam, @laststart)");
            command.Parameters.AddWithValue("@userid", Convert.ToInt32(userId));
            command.Parameters.AddWithValue("@taskname", task.TaskName);
            command.Parameters.AddWithValue("@description", task.Description);
            command.Parameters.AddWithValue("@starttime", task.StartTime);
            command.Parameters.AddWithValue("@period", task.Period);
            command.Parameters.AddWithValue("@apiid", task.ApiId);
            command.Parameters.AddWithValue("@apiparam", task.ApiParam);
            command.Parameters.AddWithValue("@laststart", task.LastStart);

           
            long? rowId = await repository.AddWithGetRowId(command);
            return (int)rowId;
        }

        public async void DeleteTaskById(int id)
        {
            SQLiteCommand command = new SQLiteCommand($"Delete From {TableName} where id = @id");
            command.Parameters.AddWithValue("@id", id);
            await repository.Delete(command);
        }
        public async void UpdateLastTime(int idTask)
        {
            SQLiteCommand command = new SQLiteCommand($"UPDATE {TableName} SET laststart = @last, count = count + 1 where id = @id");
            command.Parameters.AddWithValue("@id", idTask);
            command.Parameters.AddWithValue("@last", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            await repository.Update(command);
        }
        public async void UpdateTask(Models.Task task)
        {
            SQLiteCommand command = new SQLiteCommand($"UPDATE {TableName} SET taskname = @taskname, description = @description, apiid = @apiid, apiparam = @apiparam, " +
                $"period = @period where id = @id");
            command.Parameters.AddWithValue("@taskname", task.TaskName);
            command.Parameters.AddWithValue("@description", task.Description);
            command.Parameters.AddWithValue("@period", task.Period);
            command.Parameters.AddWithValue("@apiid", task.ApiId);
            command.Parameters.AddWithValue("@apiparam", task.ApiParam);
            command.Parameters.AddWithValue("@id", task.Id);

            await repository.Update(command);
        }
    }
}
