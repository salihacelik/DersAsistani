using System;
using System.Collections.Generic;
using DersAsistani.Data;
using DersAsistani.Models;

namespace DersAsistani.Services
{
    public class NoteService
    {
        private readonly NoteRepository _repo;

        public NoteService()
        {
            _repo = new NoteRepository();
        }

        public List<Note> GetAll()
        {
            return _repo.GetAll();
        }

        public Tuple<bool, string> Create(string title, string content)
        {
            if (string.IsNullOrWhiteSpace(title)) return Tuple.Create(false, "Konu başlığı boş olamaz.");
            if (string.IsNullOrWhiteSpace(content)) return Tuple.Create(false, "Not içeriği boş olamaz.");

            var note = new Note { Title = title.Trim(), Content = content.Trim() };

            try
            {
                _repo.Add(note);
                return Tuple.Create(true, "Not kaydedildi.");
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