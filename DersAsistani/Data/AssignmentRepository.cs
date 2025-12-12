using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;
using DersAsistani.Models;

namespace DersAsistani.Data
{
    public class AssignmentRepository
    {
        public bool Create(Assignment a)
        {
            try
            {
                using (var conn = Database.Open())
                {
                    using (var cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = @"INSERT INTO Assignments(CourseId, Title, Description, DueDate, Status, CreatedAt)
                                            VALUES(@cid, @t, @d, @due, @s, @c);";
                        cmd.Parameters.AddWithValue("@cid", a.CourseId);
                        cmd.Parameters.AddWithValue("@t", a.Title);
                        cmd.Parameters.AddWithValue("@d", a.Description ?? "");
                        cmd.Parameters.AddWithValue("@due", a.DueDate);
                        cmd.Parameters.AddWithValue("@s", a.Status);
                        cmd.Parameters.AddWithValue("@c", a.CreatedAt);
                        return cmd.ExecuteNonQuery() == 1;
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(
                    "Veritabanı hatası (Assignments Create): " + ex.Message,
                    "SQL Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Beklenmeyen hata (Assignments Create): " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public List<Assignment> GetAll()
        {
            var list = new List<Assignment>();
            try
            {
                using (var conn = Database.Open())
                {
                    using (var cmd = new SQLiteCommand("SELECT Id, CourseId, Title, Description, DueDate, Status, CreatedAt FROM Assignments;", conn))
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var a = new Assignment();
                            a.Id = r.GetInt32(0);
                            a.CourseId = r.GetInt32(1);
                            a.Title = r.GetString(2);
                            a.Description = r.IsDBNull(3) ? "" : r.GetString(3);
                            a.DueDate = r.GetString(4);
                            a.Status = r.GetString(5);
                            a.CreatedAt = r.GetString(6);
                            list.Add(a);
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(
                    "Veritabanı hatası (Assignments GetAll): " + ex.Message,
                    "SQL Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Beklenmeyen hata (Assignments GetAll): " + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return list;
        }
    }
}