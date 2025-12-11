using System;
using System.Collections.Generic;
using System.Data.SQLite;
using DersAsistani.Models;

namespace DersAsistani.Data
{
    public class AssignmentRepository
    {
        public bool Create(Assignment a)
        {
            using (var conn = new SQLiteConnection(Database.ConnectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"INSERT INTO Assignments(CourseId, Title, Description, DueDate, Status, CreatedAt)
                                        VALUES(@cid, @t, @d, @due, @s, @c);";
                    cmd.Parameters.AddWithValue("@cid", a.CourseId);
                    cmd.Parameters.AddWithValue("@t", a.Title);
                    cmd.Parameters.AddWithValue("@d", a.Description);
                    cmd.Parameters.AddWithValue("@due", a.DueDate);
                    cmd.Parameters.AddWithValue("@s", a.Status);
                    cmd.Parameters.AddWithValue("@c", a.CreatedAt);
                    return cmd.ExecuteNonQuery() == 1;
                }
            }
        }

        public List<Assignment> GetAll()
        {
            var list = new List<Assignment>();
            using (var conn = Database.Open())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT Id, CourseId, Title, Description, DueDate, Status, CreatedAt FROM Assignments;", conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var a = new Assignment();
                        a.Id = r.GetInt32(0);
                        a.CourseId = r.GetInt32(1);
                        a.Title = r.GetString(2);
                        a.Description = r.GetString(3);
                        a.DueDate = r.GetString(4);
                        a.Status = r.GetString(5);
                        a.CreatedAt = r.GetString(6);
                        list.Add(a);
                    }
                }
            }
            return list;
        }
    }
}