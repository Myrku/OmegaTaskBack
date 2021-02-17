using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mail_back.Models;

namespace mail_back.Repository
{
    public class ApiRepository
    {
        private Repository<Models.Api> repository;
        const string TableName = "Apis";
        public ApiRepository(string connectionString)
        {
            repository = new Repository<Models.Api>(connectionString, TableName);
        }

        public List<Models.Api> GetApis()
        {
            return repository.Get().Result.ToList();
        }
    }
}
