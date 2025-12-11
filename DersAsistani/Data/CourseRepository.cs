using System;
using System.Collections.Generic;
using System.Data.SQLite;
using DersAsistani.Models;

namespace DersAsistani.Data
{
    public class CourseRepository
    {
        public bool Create(Course c)
        {
            using (var conn = Database.Open())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"INSERT INTO Courses(Name, Instructor, Schedule, CreatedAt)
                                        VALUES(@n, @i, @s, @c);";
                    cmd.Parameters.AddWithValue("@n", c.Name);
                    cmd.Parameters.AddWithValue("@i", c.Instructor);
                    cmd.Parameters.AddWithValue("@s", c.Schedule);
                    cmd.Parameters.AddWithValue("@c", c.CreatedAt);
                    return cmd.ExecuteNonQuery() == 1;
                }
            }
        }

        public List<Course> GetAll()
        {
            var list = new List<Course>();
            using (var conn = Database.Open())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT Id, Name, Instructor, Schedule, CreatedAt FROM Courses;", conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var c = new Course();
                        c.Id = r.GetInt32(0);
                        c.Name = r.GetString(1);
                        c.Instructor = r.GetString(2);
                        c.Schedule = r.GetString(3);
                        c.CreatedAt = r.GetString(4);
                        list.Add(c);
                    }
                }
            }
            return list;
        }

        public bool Update(Course c)
        {
            using (var conn = Database.Open())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"UPDATE Courses
                                        SET Name=@n, Instructor=@i, Schedule=@s
                                        WHERE Id=@id;";
                    cmd.Parameters.AddWithValue("@n", c.Name);
                    cmd.Parameters.AddWithValue("@i", c.Instructor);
                    cmd.Parameters.AddWithValue("@s", c.Schedule);
                    cmd.Parameters.AddWithValue("@id", c.Id);
                    return cmd.ExecuteNonQuery() == 1;
                }
            }
        }

        public bool Delete(int id)
        {
            using (var conn = Database.Open())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM Courses WHERE Id=@id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() == 1;
                }
            }
        }
    }
}