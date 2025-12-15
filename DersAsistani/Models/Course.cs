using System;

namespace DersAsistani.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Instructor { get; set; }
        public string Day { get; set; }

        // Düzeltme: Projenin geri kalanı bunları DateTime olarak istiyor
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string Color { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Schedule özelliği ekranda göstermek için string döner
        public string Schedule
        {
            get
            {
                // Saatleri sadece saat:dakika formatında göster (Örn: 09:00)
                return $"{Day} {StartTime:HH:mm} - {EndTime:HH:mm}";
            }
            set { }
        }
    }
}