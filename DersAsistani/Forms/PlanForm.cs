using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DersAsistani.Models;
using DersAsistani.Services;

namespace DersAsistani.Forms
{
    // "public" kelimesi çok önemli, Main formun bunu görmesini sağlar.
    public class PlanForm : Form
    {
        private ListBox lstSchedules;
        private TextBox txtTitle;
        private ComboBox cmbCategory;
        private DateTimePicker dtpDate;
        private TextBox txtDescription;
        private Button btnAdd;
        private Button btnDelete;

        private ScheduleService _service;

        public PlanForm()
        {
            this.Text = "Planlarım";
            this.Width = 500;
            this.Height = 480;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            _service = new ScheduleService();
            InitializeCustomControls();
            LoadSchedules();
        }

        private void InitializeCustomControls()
        {
            lstSchedules = new ListBox { Left = 20, Top = 20, Width = 440, Height = 200 };

            txtTitle = new TextBox { Left = 20, Top = 240, Width = 200 };
            SetPlaceholder(txtTitle, "Başlık (Örn: Fizik Sınavı)");

            cmbCategory = new ComboBox { Left = 240, Top = 240, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCategory.Items.AddRange(new object[] { "Sınav", "Ders Çalışma", "Toplantı", "Etkinlik", "Diğer" });
            cmbCategory.SelectedIndex = 0;

            dtpDate = new DateTimePicker { Left = 20, Top = 280, Width = 200, Format = DateTimePickerFormat.Custom, CustomFormat = "dd.MM.yyyy HH:mm" };

            txtDescription = new TextBox { Left = 20, Top = 320, Width = 440, Height = 50, Multiline = true };
            SetPlaceholder(txtDescription, "Notlar / Açıklama...");

            btnAdd = new Button { Left = 20, Top = 385, Width = 200, Height = 40, Text = "Plan Ekle", BackColor = Color.LightSkyBlue };
            btnAdd.Click += BtnAdd_Click;

            btnDelete = new Button { Left = 260, Top = 385, Width = 200, Height = 40, Text = "Seçileni Sil", BackColor = Color.LightCoral };
            btnDelete.Click += BtnDelete_Click;

            this.Controls.Add(lstSchedules);
            this.Controls.Add(txtTitle);
            this.Controls.Add(cmbCategory);
            this.Controls.Add(dtpDate);
            this.Controls.Add(txtDescription);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnDelete);
        }

        private void LoadSchedules()
        {
            lstSchedules.Items.Clear();
            List<ScheduleItem> list = _service.GetAll();
            foreach (var item in list) lstSchedules.Items.Add(new ScheduleItemWrapper(item));
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (txtTitle.Text == "Başlık (Örn: Fizik Sınavı)" || string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Lütfen bir başlık giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string desc = txtDescription.Text == "Notlar / Açıklama..." ? "" : txtDescription.Text;
            var result = _service.Create(txtTitle.Text, cmbCategory.Text, dtpDate.Value, desc);

            if (result.Item1) { LoadSchedules(); ResetInput(txtTitle, "Başlık (Örn: Fizik Sınavı)"); ResetInput(txtDescription, "Notlar / Açıklama..."); dtpDate.Value = DateTime.Now; }
            else { MessageBox.Show(result.Item2, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lstSchedules.SelectedItem is ScheduleItemWrapper selected)
            {
                if (MessageBox.Show("Bu planı silmek istiyor musunuz?", "Sil", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _service.Delete(selected.Item.Id);
                    LoadSchedules();
                }
            }
            else { MessageBox.Show("Lütfen silmek için listeden bir plan seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private class ScheduleItemWrapper
        {
            public ScheduleItem Item { get; }
            public ScheduleItemWrapper(ScheduleItem item) { Item = item; }
            public override string ToString() { return $"[{Item.Date:dd.MM HH:mm}] {Item.Category.ToUpper()} - {Item.Title}"; }
        }

        private void SetPlaceholder(TextBox txt, string placeholder)
        {
            txt.Text = placeholder; txt.ForeColor = Color.Gray;
            txt.GotFocus += (s, e) => { if (txt.Text == placeholder) { txt.Text = ""; txt.ForeColor = Color.Black; } };
            txt.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txt.Text)) { txt.Text = placeholder; txt.ForeColor = Color.Gray; } };
        }

        private void ResetInput(TextBox txt, string placeholder) { txt.Text = placeholder; txt.ForeColor = Color.Gray; }
    }
}