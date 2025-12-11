using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DersAsistani.Data;
using DersAsistani.Models;
using DersAsistani.Services;

namespace DersAsistani.Forms
{
    public class DersForm : Form
    {
        private ListBox lstCourses = new ListBox { Left = 20, Top = 20, Width = 380, Height = 180 };
        private TextBox txtName = new TextBox { Left = 20, Top = 220, Width = 120 };
        private TextBox txtInstructor = new TextBox { Left = 150, Top = 220, Width = 120 };
        private TextBox txtSchedule = new TextBox { Left = 280, Top = 220, Width = 120 };
        private Button btnAdd = new Button { Left = 20, Top = 260, Width = 380, Text = "Ders Ekle" };

        private CourseRepository _repo;
        private CourseService _service;

        public DersForm()
        {
            this.Text = "Dersler";
            this.Width = 440;
            this.Height = 360;

            this.Controls.Add(lstCourses);
            this.Controls.Add(txtName);
            this.Controls.Add(txtInstructor);
            this.Controls.Add(txtSchedule);
            this.Controls.Add(btnAdd);

            _repo = new CourseRepository();
            _service = new CourseService(_repo);

            btnAdd.Click += BtnAdd_Click;

            LoadCourses();
        }

        private void LoadCourses()
        {
            lstCourses.Items.Clear();
            List<Course> list = _service.GetAll();
            for (int i = 0; i < list.Count; i++)
            {
                var c = list[i];
                lstCourses.Items.Add(string.Format("#{0} - {1} | {2} | {3}", c.Id, c.Name, c.Instructor, c.Schedule));
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var result = _service.Create(txtName.Text, txtInstructor.Text, txtSchedule.Text);
            bool ok = result.Item1;
            string message = result.Item2;

            MessageBox.Show(message, ok ? "Bilgi" : "Hata",
                MessageBoxButtons.OK,
                ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);

            if (ok)
            {
                LoadCourses();
                txtName.Text = "";
                txtInstructor.Text = "";
                txtSchedule.Text = "";
            }
        }
    }
}