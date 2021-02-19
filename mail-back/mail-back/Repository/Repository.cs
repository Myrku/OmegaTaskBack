using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace mail_back.Repository
{
    public class Repository<T> : IRepository<T> where T : class, new()
    {
        private SQLiteConnection db;
        readonly private string TableName;
        public Repository(string connectionString, string tableName)
        {
            this.db = new SQLiteConnection(connectionString);
            TableName = tableName;
        }
        public async Task Delete(SQLiteCommand command)
        {
            try
            {
                await db.OpenAsync();
                command.Connection = db;
                int i = await command.ExecuteNonQueryAsync();
                await db.CloseAsync();
            }
            finally
            {
                await db.CloseAsync();
            }
        }

        public async Task<IEnumerable<T>> Get()
        {
            try
            {
                await db.OpenAsync();
                SQLiteCommand command = new SQLiteCommand($"Select * From {TableName}", db);
                using(var reader = command.ExecuteReaderAsync())
                {
                    List<T> items = new List<T>();
                    if(reader.Result.HasRows)
                    {
                        while (reader.Result.Read())
                        {
                            items.Add(Map<T>(reader.Result));
                        }
                    }
                    await db.CloseAsync();
                    return items;
                }
            }
            finally
            {
                await db.CloseAsync();
            }
        }

        public async Task<T> Get(int id)
        {
            try
            {
                await db.OpenAsync();
                SQLiteCommand command = new SQLiteCommand($"Select * From {TableName} where id = {id}", db);
                using (var reader = command.ExecuteReaderAsync())
                {
                    T item = new T();
                    if (reader.Result.HasRows)
                    {
                        while (reader.Result.Read())
                        {
                            item = Map<T>(reader.Result);
                        }
                    }
                    await db.CloseAsync();
                    return item;
                }
            }
            finally
            {
                await db.CloseAsync();
            }
        }

        public async Task Add(SQLiteCommand command)
        {
            try
            {
                await db.OpenAsync();
                command.Connection = db;
                int i = await command.ExecuteNonQueryAsync();
                await db.CloseAsync();
            }
            finally
            {
                await db.CloseAsync();
            }
        }
        public async Task<long> AddWithGetRowId(SQLiteCommand command)
        {
            try
            {
                await db.OpenAsync();
                command.Connection = db;
                await command.ExecuteNonQueryAsync();
                long rowId = db.LastInsertRowId;
                await db.CloseAsync();
                return rowId;
            }
            finally
            {
                await db.CloseAsync();
            }
        }
        public async Task Update(SQLiteCommand command)
        {
            try
            {
                await db.OpenAsync();
                command.Connection = db;
                int i = await command.ExecuteNonQueryAsync();
                await db.CloseAsync();
            }
            finally
            {
                await db.CloseAsync();
            }
        }

        protected T Map<entity>(IDataRecord record)
        {
            var objT = Activator.CreateInstance<T>();
            foreach (var property in typeof(entity).GetProperties())
            {
                if (!record.IsDBNull(record.GetOrdinal(property.Name)) && property.PropertyType != typeof(int))
                {
                    property.SetValue(objT, record[property.Name]);
                }
                else
                {
                    property.SetValue(objT, Convert.ToInt32(record[property.Name]));
                }
            }

            return objT;

        }

        public string GetHashPassword(string password)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha512.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes);
                return hash;
            }
        }
    }
}
