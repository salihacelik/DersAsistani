// Data/Database.cs
using System;
using System.Data.SQLite;
using System.IO;

namespace DersAsistani.Data
{
    public static class Database
    {
        public static string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ders_asistani.db");
        public static string ConnectionString = "Data Source=" + DbPath + ";Version=3;";

        public static SQLiteConnection Open()
        {
            var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            using (var pragma = new SQLiteCommand("PRAGMA foreign_keys = ON;", conn)) { pragma.ExecuteNonQuery(); }
            using (var journal = new SQLiteCommand("PRAGMA journal_mode = WAL;", conn)) { journal.ExecuteNonQuery(); }
            using (var syn = new SQLiteCommand("PRAGMA synchronous = NORMAL;", conn)) { syn.ExecuteNonQuery(); }
            return conn;
        }

        public static void EnsureCreated()
        {
            if (!File.Exists(DbPath)) SQLiteConnection.CreateFile(DbPath);
            using (var conn = Open())
            using (var cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Users(
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  Username TEXT NOT NULL UNIQUE,
  PasswordHash TEXT NOT NULL,
  FullName TEXT NOT NULL,
  CreatedAt TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS Courses(
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  Name TEXT NOT NULL UNIQUE,
  Description TEXT DEFAULT '',
  CreatedAt TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS Assignments(
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  CourseId INTEGER NOT NULL,
  Title TEXT NOT NULL,
  Description TEXT DEFAULT '',
  DueDate TEXT NOT NULL,
  Status TEXT NOT NULL DEFAULT 'Bekliyor',
  CreatedAt TEXT NOT NULL,
  FOREIGN KEY (CourseId) REFERENCES Courses(Id) ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS ScheduleItems(
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  Title TEXT NOT NULL,
  Details TEXT DEFAULT '',
  StartTime TEXT NOT NULL,
  EndTime TEXT NOT NULL,
  Category TEXT NOT NULL,
  CreatedAt TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS Grades(
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  CourseId INTEGER NOT NULL,
  ExamType TEXT NOT NULL,
  Score INTEGER NOT NULL,
  CreatedAt TEXT NOT NULL,
  FOREIGN KEY (CourseId) REFERENCES Courses(Id) ON DELETE CASCADE
);
CREATE UNIQUE INDEX IF NOT EXISTS IX_Users_Username ON Users(Username);
CREATE INDEX IF NOT EXISTS IX_Assignments_CourseId ON Assignments(CourseId);
CREATE INDEX IF NOT EXISTS IX_Grades_CourseId ON Grades(CourseId);
CREATE INDEX IF NOT EXISTS IX_ScheduleItems_StartTime ON ScheduleItems(StartTime);
";
                cmd.ExecuteNonQuery();
            }
        }
    }
}