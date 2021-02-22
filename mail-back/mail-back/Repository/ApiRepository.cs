using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using mail_back.Models;

namespace mail_back.Repository
{
    public class ApiRepository
    {
        private Repository<Models.Api> repository;
        private const string TableName = "Apis";
        public ApiRepository(string connectionString)
        {
            repository = new Repository<Models.Api>(connectionString);
        }

        public List<Models.Api> GetApis()
        {
            SQLiteCommand command = new SQLiteCommand
            {
                CommandText = $"Select Id, ApiName from {TableName}"
            };
            return repository.Get(command).Result.ToList();
        }
    }
}
