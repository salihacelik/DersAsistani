using System;
using System.Collections.Generic;
using DersAsistani.Data;
using DersAsistani.Models;

namespace DersAsistani.Services
{
    public class AssignmentService
    {
        private readonly AssignmentRepository _repo;

        public AssignmentService()
        {
            _repo = new AssignmentRepository();
        }

        // HATA ÇÖZÜMÜ 1: "GetAll" metodu eklendi
        public List<Assignment> GetAll()
        {
            return _repo.GetAll();
        }

        // HATA ÇÖZÜMÜ 2: "Create" metodu eklendi
        public Tuple<bool, string> Create(string courseName, string description, DateTime dueDate)
        {
            if (string.IsNullOrWhiteSpace(courseName)) return Tuple.Create(false, "Ders adı boş olamaz.");
            // Açıklama boş olabilir, sorun yok

            var assignment = new Assignment
            {
                CourseName = courseName.Trim(),
                Description = description?.Trim() ?? "",
                DueDate = dueDate,
                IsCompleted = false
            };

            try
            {
                _repo.Create(assignment);
                return Tuple.Create(true, "Ödev başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, "Veritabanı hatası: " + ex.Message);
            }
        }

        // HATA ÇÖZÜMÜ 3: "Delete" metodu eklendi
        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        // HATA ÇÖZÜMÜ 4: "ToggleStatus" metodu eklendi (Checkbox için)
        public void ToggleStatus(int id, bool isCompleted)
        {
            _repo.UpdateStatus(id, isCompleted);
        }
    }
}