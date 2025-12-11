using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DersAsistani.Data;
using DersAsistani.Models;
using DersAsistani.Utils;

namespace DersAsistani.Forms
{
    public class GradesForm : Form
    {
        private ComboBox cmbCourse = new ComboBox { Left = 20, Top = 20, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        private ComboBox cmbExamType = new ComboBox { Left = 240, Top = 20, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
        private NumericUpDown numScore = new NumericUpDown { Left = 380, Top = 20, Width = 80, Minimum = 0, Maximum = 100 };
        private Button btnAdd = new Button { Left = 480, Top = 20, Width = 100, Text = "Not Ekle" };
        private DataGridView grid = new DataGridView { Left = 20, Top = 60, Width = 560, Height = 200 };
        private Label lblAverage = new Label { Left = 20, Top = 280, AutoSize = true };

        private GradesRepository _repo;
        private CourseRepository _courseRepo;

        public GradesForm()
        {
            this.Text = "Sınavlar";
            this.Width = 620;
            this.Height = 360;

            this.Controls.Add(cmbCourse);
            this.Controls.Add(cmbExamType);
            this.Controls.Add(numScore);
            this.Controls.Add(btnAdd);
            this.Controls.Add(grid);
            this.Controls.Add(lblAverage);
            UIHelper.ApplyFormTheme(this);
            UIHelper.StyleButton(btnAdd);
            UIHelper.StyleGrid(grid);


            cmbExamType.Items.AddRange(new string[] { "Vize", "Final", "Quiz" });

            _repo = new GradesRepository();
            _courseRepo = new CourseRepository();

            btnAdd.Click += BtnAdd_Click;

            LoadCourses();
            LoadGrades();
        }

        private void LoadCourses()
        {
            cmbCourse.Items.Clear();
            var courses = _courseRepo.GetAll();
            for (int i = 0; i < courses.Count; i++)
            {
                cmbCourse.Items.Add(courses[i]);
            }
            cmbCourse.DisplayMember = "Name";
            cmbCourse.ValueMember = "Id";
        }

        private void LoadGrades()
        {
            grid.Rows.Clear();
            grid.Columns.Clear();
            grid.Columns.Add("CourseId", "Ders");
            grid.Columns.Add("ExamType", "Sınav Türü");
            grid.Columns.Add("Score", "Puan");
            grid.Columns.Add("CreatedAt", "Tarih");

            var list = _repo.GetAll();
            int total = 0;
            int count = 0;

            for (int i = 0; i < list.Count; i++)
            {
                var g = list[i];
                grid.Rows.Add(g.CourseId, g.ExamType, g.Score, g.CreatedAt);
                total += g.Score;
                count++;
            }

            lblAverage.Text = count > 0 ? "Ortalama Not: " + (total / count) : "Henüz not yok.";
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (cmbCourse.SelectedItem == null || cmbExamType.SelectedItem == null)
            {
                MessageBox.Show("Lütfen ders ve sınav türü seçin.");
                return;
            }

            var course = (Course)cmbCourse.SelectedItem;

            var g = new Grade();
            g.CourseId = course.Id;
            g.ExamType = cmbExamType.SelectedItem.ToString();
            g.Score = (int)numScore.Value;
            g.CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            bool ok = _repo.Create(g);

            MessageBox.Show(ok ? "Not eklendi." : "Kayıt sırasında hata oluştu.",
                ok ? "Bilgi" : "Hata",
                MessageBoxButtons.OK,
                ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);

            if (ok) LoadGrades();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // GradesForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "GradesForm";
            this.Load += new System.EventHandler(this.GradesForm_Load);
            this.ResumeLayout(false);

        }

        private void GradesForm_Load(object sender, EventArgs e)
        {

        }
    }
}