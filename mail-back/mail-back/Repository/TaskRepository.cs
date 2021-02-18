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
            repository = new Repository<Models.Task>(connectionString, TableName);
        }

        public List<Models.Task> GetTasks()
        {
            return repository.Get().Result.ToList();
        }
        public List<Models.Task> GetTasksByUserId(string userId)
        {
            return repository.Get().Result.Where(x => x.userid == Convert.ToInt32(userId)).ToList();
        }
        public async void AddTask(Models.Task task, string userId)
        {
            SQLiteCommand command = new SQLiteCommand($"Insert into {TableName}(userid, taskname, description, starttime, period, apiid, apiparam, laststart) " +
                $"Values(@userid, @taskname, @description, @starttime, @period, @apiid, @apiparam, @laststart)");
            command.Parameters.AddWithValue("@userid", Convert.ToInt32(userId));
            command.Parameters.AddWithValue("@taskname",task.taskname);
            command.Parameters.AddWithValue("@description", task.description);
            command.Parameters.AddWithValue("@starttime", task.starttime);
            command.Parameters.AddWithValue("@period", task.period);
            command.Parameters.AddWithValue("@apiid", task.apiid);
            command.Parameters.AddWithValue("@apiparam", task.apiparam);
            command.Parameters.AddWithValue("@laststart", task.laststart);
            await repository.Add(command);

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
    }
}
