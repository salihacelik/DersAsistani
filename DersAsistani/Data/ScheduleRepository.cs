using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms; // MessageBox için gerekli
using DersAsistani.Models;

namespace DersAsistani.Data
{
    public class ScheduleRepository
    {
        public bool Create(ScheduleItem p)
        {
            try
            {
                using (var conn = Database.Open())
                {
                    using (var cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = @"
INSERT INTO ScheduleItems(Title, Details, StartTime, EndTime, Category, CreatedAt)
VALUES(@t, @d, @s, @e, @c, @created);";
                        cmd.Parameters.AddWithValue("@t", p.Title);
                        cmd.Parameters.AddWithValue("@d", p.Details ?? "");
                        cmd.Parameters.AddWithValue("@s", p.StartTime);
                        cmd.Parameters.AddWithValue("@e", p.EndTime);
                        cmd.Parameters.AddWithValue("@c", p.Category);
                        cmd.Parameters.AddWithValue("@created", p.CreatedAt);

                        return cmd.ExecuteNonQuery() == 1;
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(
                    "Veritabanı hatası (ScheduleItems Create): " + ex.Message,
                    "SQL Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public List<ScheduleItem> GetAll()
        {
            var list = new List<ScheduleItem>();
            try
            {
                using (var conn = Database.Open())
                using (var cmd = new SQLiteCommand(
                    "SELECT Id, Title, Details, StartTime, EndTime, Category, CreatedAt FROM ScheduleItems ORDER BY StartTime;", conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var p = new ScheduleItem();
                        p.Id = r.GetInt32(0);
                        p.Title = r.GetString(1);
                        p.Details = r.GetString(2);
                        p.StartTime = r.GetString(3);
                        p.EndTime = r.GetString(4);
                        p.Category = r.GetString(5);
                        p.CreatedAt = r.GetString(6);
                        list.Add(p);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(
                    "Veritabanı hatası (ScheduleItems GetAll): " + ex.Message,
                    "SQL Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return list;
        }
    }
}