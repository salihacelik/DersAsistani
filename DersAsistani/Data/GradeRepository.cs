using System;
using System.Collections.Generic;
using System.Data.SQLite;
using DersAsistani.Models;

namespace DersAsistani.Data
{
    public class GradesRepository
    {
        public bool Create(Grade g)
        {
            try
            {
                using (var conn = new SQLiteConnection(Database.ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = @"INSERT INTO Grades(CourseId, ExamType, Score, CreatedAt)
                                            VALUES(@cid, @exam, @score, @created);";
                        cmd.Parameters.AddWithValue("@cid", g.CourseId);
                        cmd.Parameters.AddWithValue("@exam", g.ExamType);
                        cmd.Parameters.AddWithValue("@score", g.Score);
                        cmd.Parameters.AddWithValue("@created", g.CreatedAt);
                        return cmd.ExecuteNonQuery() == 1;
                    }
                }
            }
            catch (SQLiteException ex)
            {
                System.Windows.Forms.MessageBox.Show("Veritabanı hatası (Grades): " + ex.Message,
                    "SQL Hatası", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }

        public List<Grade> GetAll()
        {
            var list = new List<Grade>();
            using (var conn = Database.Open())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT Id, CourseId, ExamType, Score, CreatedAt FROM Grades;", conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var g = new Grade();
                        g.Id = r.GetInt32(0);
                        g.CourseId = r.GetInt32(1);
                        g.ExamType = r.GetString(2);
                        g.Score = r.GetInt32(3);
                        g.CreatedAt = r.GetString(4);
                        list.Add(g);
                    }
                }
            }
            return list;
        }
    }
}