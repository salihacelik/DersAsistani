using System;

namespace DersAsistani.Models
{
    public class Assignment
    {
        public int Id { get; set; }

        // Hata Alan Yer: Artık "CourseId" değil "CourseName" kullanıyoruz
        public string CourseName { get; set; }

        public string Description { get; set; }

        // Hata Alan Yer: DateTime formatı
        public DateTime DueDate { get; set; }

        // Hata Alan Yer: Onay kutusu için gerekli
        public bool IsCompleted { get; set; }
    }
}