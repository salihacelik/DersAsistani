using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DersAsistani.Data;
using DersAsistani.Models;

namespace DersAsistani.Forms
{
    public class OdevForm : Form
    {
        private ListBox lstAssignments = new ListBox { Left = 20, Top = 20, Width = 380, Height = 180 };
        private TextBox txtTitle = new TextBox { Left = 20, Top = 220, Width = 120 };
        private TextBox txtDescription = new TextBox { Left = 150, Top = 220, Width = 120 };
        private TextBox txtDueDate = new TextBox { Left = 280, Top = 220, Width = 120 };
        private Button btnAdd = new Button { Left = 20, Top = 260, Width = 380, Text = "Ödev Ekle" };

        private AssignmentRepository _repo;

        public OdevForm()
        {
            this.Text = "Ödevler";
            this.Width = 440;
            this.Height = 360;

            this.Controls.Add(lstAssignments);
            this.Controls.Add(txtTitle);
            this.Controls.Add(txtDescription);
            this.Controls.Add(txtDueDate);
            this.Controls.Add(btnAdd);

            _repo = new AssignmentRepository();

            btnAdd.Click += BtnAdd_Click;

            LoadAssignments();
        }

        private void LoadAssignments()
        {
            lstAssignments.Items.Clear();
            List<Assignment> list = _repo.GetAll();
            for (int i = 0; i < list.Count; i++)
            {
                var a = list[i];
                lstAssignments.Items.Add(string.Format("#{0} - {1} | {2} | {3} | {4}",
                    a.Id, a.Title, a.Description, a.DueDate, a.Status));
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var a = new Assignment();
            a.CourseId = 1; // Şimdilik sabit, ileride ders seçimi ekleyeceğiz
            a.Title = txtTitle.Text.Trim();
            a.Description = txtDescription.Text.Trim();
            a.DueDate = txtDueDate.Text.Trim();
            a.Status = "Bekliyor";
            a.CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            bool ok = _repo.Create(a);

            MessageBox.Show(ok ? "Ödev eklendi." : "Kayıt sırasında hata oluştu.",
                ok ? "Bilgi" : "Hata",
                MessageBoxButtons.OK,
                ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);

            if (ok)
            {
                LoadAssignments();
                txtTitle.Text = "";
                txtDescription.Text = "";
                txtDueDate.Text = "";
            }
        }
    }
}