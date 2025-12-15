using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using DersAsistani.Models;

namespace DersAsistani.Data
{
    public class CourseRepository
    {
        private string _connectionString;

        public CourseRepository()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DersAsistani.db");
            _connectionString = $"Data Source={dbPath};Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DersAsistani.db")))
            {
                SQLiteConnection.CreateFile("DersAsistani.db");
            }

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    CREATE TABLE IF NOT EXISTS Courses (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Instructor TEXT,
                        Day TEXT,
                        StartTime TEXT,
                        EndTime TEXT,
                        Color TEXT
                    )";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Course> GetAll()
        {
            var list = new List<Course>();

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Courses";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime start = DateTime.Now;
                            DateTime end = DateTime.Now;

                            DateTime.TryParse(reader["StartTime"].ToString(), out start);
                            DateTime.TryParse(reader["EndTime"].ToString(), out end);

                            list.Add(new Course
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Instructor = reader["Instructor"] != DBNull.Value ? reader["Instructor"].ToString() : "",
                                Day = reader["Day"] != DBNull.Value ? reader["Day"].ToString() : "",
                                StartTime = start,
                                EndTime = end,
                                Color = reader["Color"] != DBNull.Value ? reader["Color"].ToString() : "#FFFFFF",
                                CreatedAt = DateTime.Now
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void Create(Course course)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Courses (Name, Instructor, Day, StartTime, EndTime, Color) VALUES (@Name, @Instructor, @Day, @StartTime, @EndTime, @Color)";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", course.Name);
                    cmd.Parameters.AddWithValue("@Instructor", course.Instructor ?? "");
                    cmd.Parameters.AddWithValue("@Day", course.Day ?? "");
                    cmd.Parameters.AddWithValue("@StartTime", course.StartTime.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@EndTime", course.EndTime.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@Color", course.Color ?? "#FFFFFF");

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // SİLME METODU BURADA
        public void Delete(int id)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM Courses WHERE Id = @Id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}