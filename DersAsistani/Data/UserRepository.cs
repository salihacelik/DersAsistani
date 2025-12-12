using System;
using System.Data.SQLite;
using DersAsistani.Models;

namespace DersAsistani.Data
{
    public class UserRepository
    {
        public bool Create(User user)
        {
            using (var conn = Database.Open())
            {
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"INSERT INTO Users(Username, PasswordHash, FullName, CreatedAt)
                                        VALUES(@u, @p, @f, @c);";
                    cmd.Parameters.AddWithValue("@u", user.Username);
                    cmd.Parameters.AddWithValue("@p", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@f", user.FullName);
                    cmd.Parameters.AddWithValue("@c", user.CreatedAt);
                    return cmd.ExecuteNonQuery() == 1;
                }
            }
        }

        public User GetByUsername(string username)
        {
            using (var conn = Database.Open())
            {
                using (var cmd = new SQLiteCommand("SELECT Id, Username, PasswordHash, FullName, CreatedAt FROM Users WHERE Username=@u;", conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;

                        var user = new User();
                        user.Id = reader.GetInt32(0);
                        user.Username = reader.GetString(1);
                        user.PasswordHash = reader.GetString(2);
                        user.FullName = reader.GetString(3);
                        user.CreatedAt = reader.GetString(4);
                        return user;
                    }
                }
            }
        }

        public bool Exists(string username)
        {
            using (var conn = Database.Open())
            {
                using (var cmd = new SQLiteCommand("SELECT COUNT(1) FROM Users WHERE Username=@u;", conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
    }
}