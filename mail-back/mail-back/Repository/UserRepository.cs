using mail_back.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace mail_back.Repository
{
    public class UserRepository : Repository<User>
    {
        public UserRepository(string connectionString) : base(connectionString, "Users")
        {
        }

        public List<User> GetUsers()
        {
            return this.Get().Result.ToList();
        }

        public User GetUserById(int id)
        {
            return this.Get(id).Result;
        }

        public int InsertUser(User user)
        {
            SQLiteCommand command = new SQLiteCommand("Insert into Users Values(@id, @username, @email, @password, @idrole)");
            command.Parameters.AddWithValue("@id", user.id);
            command.Parameters.AddWithValue("@username", user.username);
            command.Parameters.AddWithValue("@email", user.email);
            command.Parameters.AddWithValue("@password", GetHashPassword(user.password));
            command.Parameters.AddWithValue("@idrole", user.idrole);

            return this.Insert(command).Result;
        }
    }
}
