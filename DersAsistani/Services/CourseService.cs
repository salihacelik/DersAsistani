using System;
using System.Collections.Generic;
using DersAsistani.Data;
using DersAsistani.Models;

namespace DersAsistani.Services
{
    public class CourseService
    {
        private readonly CourseRepository _repo;

        public CourseService(CourseRepository repo)
        {
            _repo = repo;
        }

        public Tuple<bool, string> Create(string name, string instructor, string schedule)
        {
            if (string.IsNullOrWhiteSpace(name)) return Tuple.Create(false, "Ders adı boş olamaz.");
            // Diğer kontroller isteğe bağlı gevşetilebilir

            var c = new Course();
            c.Name = name.Trim();
            c.Instructor = instructor.Trim();
            c.CreatedAt = DateTime.Now;
            c.Color = "#4cc9f0";

            ParseSchedule(schedule, c);

            try
            {
                _repo.Create(c);
                return Tuple.Create(true, "Ders başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, "Veritabanı hatası: " + ex.Message);
            }
        }

        public List<Course> GetAll()
        {
            return _repo.GetAll();
        }

        // HATA ALDIĞINIZ KISIM BURADAYDI - ŞİMDİ DOĞRU YERDE
        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        private void ParseSchedule(string rawSchedule, Course c)
        {
            try
            {
                var parts = rawSchedule.Trim().Split(' ');

                if (parts.Length > 0) c.Day = parts[0];
                else c.Day = "Belirsiz";

                if (parts.Length > 1 && DateTime.TryParse(parts[1], out DateTime start))
                    c.StartTime = start;
                else
                    c.StartTime = DateTime.Now;

                if (parts.Length > 3 && DateTime.TryParse(parts[3], out DateTime end))
                    c.EndTime = end;
                else
                    c.EndTime = c.StartTime.AddHours(1);
            }
            catch
            {
                c.Day = rawSchedule;
                c.StartTime = DateTime.Now;
                c.EndTime = DateTime.Now.AddHours(1);
            }
        }
    }
}