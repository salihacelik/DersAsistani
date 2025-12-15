using System;
using System.Collections.Generic;
using DersAsistani.Data;
using DersAsistani.Models;

namespace DersAsistani.Services
{
    public class ScheduleService
    {
        private readonly ScheduleRepository _repo;

        public ScheduleService()
        {
            _repo = new ScheduleRepository();
        }

        public List<ScheduleItem> GetAll()
        {
            return _repo.GetAll();
        }

        public Tuple<bool, string> Create(string title, string category, DateTime date, string description)
        {
            if (string.IsNullOrWhiteSpace(title)) return Tuple.Create(false, "Başlık boş olamaz.");

            var item = new ScheduleItem
            {
                Title = title.Trim(),
                Category = category,
                Date = date,
                Description = description?.Trim() ?? ""
            };

            try
            {
                _repo.Add(item);
                return Tuple.Create(true, "Plan eklendi.");
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, "Hata: " + ex.Message);
            }
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }
    }
}