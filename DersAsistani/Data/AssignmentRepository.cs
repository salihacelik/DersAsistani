using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using DersAsistani.Models;

namespace DersAsistani.Data
{
    public class AssignmentRepository
    {
        private string _connectionString;

        public AssignmentRepository()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DersAsistani.db");
            _connectionString = $"Data Source={dbPath};Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                // Veritabanı yapısını yeni Modele göre ayarlıyoruz
                string sql = @"
                    CREATE TABLE IF NOT EXISTS Assignments (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        CourseName TEXT,
                        Description TEXT,
                        DueDate TEXT,
                        IsCompleted INTEGER DEFAULT 0 
                    )";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // HATA ÇÖZÜMÜ: Metodun adı "Add" değil "Create" yapıldı.
        public void Create(Assignment assignment)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Assignments (CourseName, Description, DueDate, IsCompleted) VALUES (@CourseName, @Description, @DueDate, 0)";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseName", assignment.CourseName);
                    cmd.Parameters.AddWithValue("@Description", assignment.Description);

                    // HATA ÇÖZÜMÜ: DateTime -> String dönüşümü elle yapıldı
                    cmd.Parameters.AddWithValue("@DueDate", assignment.DueDate.ToString("yyyy-MM-dd HH:mm"));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Assignment> GetAll()
        {
            var list = new List<Assignment>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Assignments ORDER BY DueDate ASC";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // HATA ÇÖZÜMÜ: String -> DateTime dönüşümü
                            DateTime dueDate = DateTime.Now;
                            DateTime.TryParse(reader["DueDate"].ToString(), out dueDate);

                            list.Add(new Assignment
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                CourseName = reader["CourseName"].ToString(), // CourseName hatası çözümü
                                Description = reader["Description"].ToString(),
                                DueDate = dueDate,
                                IsCompleted = Convert.ToInt32(reader["IsCompleted"]) == 1 // IsCompleted hatası çözümü
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void Delete(int id)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM Assignments WHERE Id = @Id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateStatus(int id, bool isCompleted)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "UPDATE Assignments SET IsCompleted = @Status WHERE Id = @Id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", isCompleted ? 1 : 0);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}