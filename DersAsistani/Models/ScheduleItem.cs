using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DersAsistani.Models
{
    public class ScheduleItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Category { get; set; }
        public string CreatedAt { get; set; }
    }
}