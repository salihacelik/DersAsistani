using System;
using System.Windows.Forms;
using DersAsistani.Data;

namespace DersAsistani.Forms
{
    public class RaporForm : Form
    {
        private Label lblCourses = new Label { Left = 20, Top = 20, AutoSize = true };
        private Label lblAssignments = new Label { Left = 20, Top = 60, AutoSize = true };
        private Label lblCompleted = new Label { Left = 20, Top = 100, AutoSize = true };

        private CourseRepository _courseRepo;
        private AssignmentRepository _assignmentRepo;

        public RaporForm()
        {
            this.Text = "Raporlar";
            this.Width = 400;
            this.Height = 200;

            this.Controls.Add(lblCourses);
            this.Controls.Add(lblAssignments);
            this.Controls.Add(lblCompleted);

            _courseRepo = new CourseRepository();
            _assignmentRepo = new AssignmentRepository();

            LoadStats();
        }

        private void LoadStats()
        {
            int courseCount = _courseRepo.GetAll().Count;
            int assignmentCount = _assignmentRepo.GetAll().Count;
            int completedCount = 0;

            foreach (var a in _assignmentRepo.GetAll())
            {
                if (a.IsCompleted)
                {
                    completedCount++;
                }
            }

            lblCourses.Text = "Toplam Ders Sayısı: " + courseCount;
            lblAssignments.Text = "Toplam Ödev Sayısı: " + assignmentCount;
            lblCompleted.Text = "Tamamlanan Ödevler: " + completedCount;
        }
    }
}