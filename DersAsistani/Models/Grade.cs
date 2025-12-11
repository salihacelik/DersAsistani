using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DersAsistani.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string ExamType { get; set; }
        public int Score { get; set; }
        public string CreatedAt { get; set; }
    }
}