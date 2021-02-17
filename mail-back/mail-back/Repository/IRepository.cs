using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace mail_back.Repository
{
    interface IRepository <T> where T : class, new()
    {
        Task<IEnumerable<T>> Get();
        Task<T> Get(int id);
        Task Add(SQLiteCommand command);
        Task Update(SQLiteCommand command);
        Task Delete(SQLiteCommand command);
    }
}
