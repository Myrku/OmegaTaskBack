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
        public UserRepository(string connectionString)
        {
            repository = new Repository<User>(connectionString, "Users");
        }

        public List<User> GetUsers()
        {
            return repository.Get().Result.ToList();
        }

        public User GetUserById(int id)
        {
            return repository.Get(id).Result;
        }

        public async void InsertUser(User user)
        {
            SQLiteCommand command = new SQLiteCommand("Insert into Users(username, email, password) Values(@username, @email, @password)");
            command.Parameters.AddWithValue("@username", user.username);
            command.Parameters.AddWithValue("@email", user.email);
            command.Parameters.AddWithValue("@password", repository.GetHashPassword(user.password));
            await repository.Add(command);
        }
    }
}
