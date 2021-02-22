using mail_back.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace mail_back.Repository
{
    public class UserRepository
    {
        Repository<User> repository;
        private const string TableName = "Users";
        public UserRepository(string connectionString)
        {
            repository = new Repository<User>(connectionString);
        }

        public List<User> GetUsers()
        {
            SQLiteCommand command = new SQLiteCommand
            {
                CommandText = $"Select Id, UserName, Email, Password, IdRole from {TableName}"
            };
            return repository.Get(command).Result.ToList();
        }

        public User GetUserById(int id)
        {
            SQLiteCommand command = new SQLiteCommand
            {
                CommandText = $"Select Id, UserName, Email, Password, IdRole from {TableName} where id = @id"
            };
            command.Parameters.AddWithValue("@id", id);
            return repository.Get(command).Result.FirstOrDefault();
        }

        public async void InsertUser(User user)
        {
            SQLiteCommand command = new SQLiteCommand("Insert into Users(username, email, password) Values(@username, @email, @password)");
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@password", repository.GetHashPassword(user.Password));
            await repository.Add(command);
        }
    }
}
