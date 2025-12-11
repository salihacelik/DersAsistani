using System;
using System.Collections.Generic;
using DersAsistani.Data;
using DersAsistani.Models;
using DersAsistani.Utils;

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
            if (!Validation.IsNonEmpty(name)) return Tuple.Create(false, "Ders adı boş olamaz.");
            if (!Validation.IsNonEmpty(instructor)) return Tuple.Create(false, "Eğitmen adı boş olamaz.");
            if (!Validation.IsNonEmpty(schedule)) return Tuple.Create(false, "Program bilgisi boş olamaz.");

            var c = new Course();
            c.Name = name.Trim();
            c.Instructor = instructor.Trim();
            c.Schedule = schedule.Trim();
            c.CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            var ok = _repo.Create(c);
            return ok ? Tuple.Create(true, "Ders eklendi.") : Tuple.Create(false, "Kayıt sırasında hata oluştu.");
        }

        public List<Course> GetAll()
        {
            return _repo.GetAll();
        }
    }
}