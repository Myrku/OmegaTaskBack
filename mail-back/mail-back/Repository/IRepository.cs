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
        Task<int> Insert(SQLiteCommand command);
        Task<int> Update(T entity);
        Task<int> Delete(T entity);
    }
}
