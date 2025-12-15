using System;

namespace DersAsistani.Models
{
    public class ScheduleItem
    {
        public int Id { get; set; }
        public string Title { get; set; }       // Başlık (Örn: Matematik Sınavı)
        public string Category { get; set; }    // Kategori (Sınav, Toplantı, Aktivite)
        public DateTime Date { get; set; }      // Ne zaman?
        public string Description { get; set; } // Detaylar
    }
}