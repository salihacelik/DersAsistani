using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DersAsistani.Data;
using DersAsistani.Models;


namespace DersAsistani.Forms
{
    public class PlanForm : Form
    {
        private DataGridView grid = new DataGridView { Left = 20, Top = 20, Width = 500, Height = 200 };
        private TextBox txtTitle = new TextBox { Left = 20, Top = 240, Width = 120 };
        private TextBox txtDetails = new TextBox { Left = 150, Top = 240, Width = 120 };
        private DateTimePicker dtStart = new DateTimePicker { Left = 280, Top = 240, Width = 120, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm" };
        private DateTimePicker dtEnd = new DateTimePicker { Left = 410, Top = 240, Width = 120, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm" };
        private ComboBox cmbCategory = new ComboBox { Left = 20, Top = 280, Width = 120 };
        private Button btnAdd = new Button { Left = 150, Top = 280, Width = 380, Text = "Plan Ekle" };

        private ScheduleRepository _repo;

        public PlanForm()
        {
            this.Text = "Planlarım";
            this.Width = 560;
            this.Height = 380;

            this.Controls.Add(grid);
            this.Controls.Add(txtTitle);
            this.Controls.Add(txtDetails);
            this.Controls.Add(dtStart);
            this.Controls.Add(dtEnd);
            this.Controls.Add(cmbCategory);
            this.Controls.Add(btnAdd);

            cmbCategory.Items.AddRange(new string[] { "Ders", "Ödev", "Sınav", "Kişisel" });

            _repo = new ScheduleRepository();

            btnAdd.Click += BtnAdd_Click;

            LoadPlans();
        }

        private void LoadPlans()
        {
            grid.Rows.Clear();
            grid.Columns.Clear();
            grid.Columns.Add("Title", "Başlık");
            grid.Columns.Add("Details", "Detay");
            grid.Columns.Add("StartTime", "Başlangıç");
            grid.Columns.Add("EndTime", "Bitiş");
            grid.Columns.Add("Category", "Kategori");

            List<ScheduleItem> list = _repo.GetAll();
            for (int i = 0; i < list.Count; i++)
            {
                var p = list[i];
                grid.Rows.Add(p.Title, p.Details, p.StartTime, p.EndTime, p.Category);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Başlık ve kategori zorunlu.");
                return;
            }

            var p = new ScheduleItem();
            p.Title = txtTitle.Text.Trim();
            p.Details = txtDetails.Text.Trim();
            p.StartTime = dtStart.Value.ToString("yyyy-MM-dd HH:mm:ss");
            p.EndTime = dtEnd.Value.ToString("yyyy-MM-dd HH:mm:ss");
            p.Category = cmbCategory.SelectedItem.ToString();
            p.CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            bool ok = _repo.Create(p);

            MessageBox.Show(ok ? "Plan eklendi." : "Kayıt sırasında hata oluştu.",
                ok ? "Bilgi" : "Hata",
                MessageBoxButtons.OK,
                ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);

            if (ok)
            {
                LoadPlans();
                txtTitle.Text = "";
                txtDetails.Text = "";
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PlanForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "PlanForm";
            this.ResumeLayout(false);

        }

        
    }
}