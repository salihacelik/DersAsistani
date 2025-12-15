using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DersAsistani.Models;
using DersAsistani.Services; // Repository yerine Service kullanmak daha iyidir

namespace DersAsistani.Forms
{
    public class OdevForm : Form
    {
        // Kontroller
        private CheckedListBox lstAssignments; // Tik atılabilen liste
        private TextBox txtCourseName;         // Ders Adı (Eski: Title)
        private TextBox txtDescription;        // Açıklama
        private DateTimePicker dtpDueDate;     // Tarih Seçici (Eski: TextBox)
        private Button btnAdd;
        private Button btnDelete;

        // Service katmanını kullanıyoruz (Repository'yi sarmalar)
        private AssignmentService _service;

        // Bu değişkeni check olayını kontrol etmek için kullanacağız
        private bool _isLoading = false;

        public OdevForm()
        {
            // Form Ayarları
            this.Text = "Ödev Takibi";
            this.Width = 500;
            this.Height = 450;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            _service = new AssignmentService();
            InitializeCustomControls();
            LoadAssignments();
        }

        private void InitializeCustomControls()
        {
            // 1. Onay Kutulu Liste
            lstAssignments = new CheckedListBox
            {
                Left = 20,
                Top = 20,
                Width = 440,
                Height = 220,
                CheckOnClick = true // Tek tıklamayla işaretlensin
            };
            lstAssignments.ItemCheck += LstAssignments_ItemCheck;

            // 2. Veri Giriş Alanları
            txtCourseName = new TextBox { Left = 20, Top = 260, Width = 140 };

            // Tarih Seçici (Eski TextBox yerine bunu kullanıyoruz)
            dtpDueDate = new DateTimePicker
            {
                Left = 170,
                Top = 260,
                Width = 140,
                Format = DateTimePickerFormat.Short
            };

            txtDescription = new TextBox
            {
                Left = 20,
                Top = 290,
                Width = 290,
                Height = 60,
                Multiline = true
            };

            // Placeholder (Silik Yazı) Ayarları
            SetPlaceholder(txtCourseName, "Ders Adı");
            SetPlaceholder(txtDescription, "Ödev içeriği / notlar...");

            // 3. Butonlar
            btnAdd = new Button
            {
                Left = 320,
                Top = 260,
                Width = 140,
                Height = 40,
                Text = "Ödev Ekle",
                BackColor = Color.LightGreen
            };
            btnAdd.Click += BtnAdd_Click;

            btnDelete = new Button
            {
                Left = 320,
                Top = 310,
                Width = 140,
                Height = 40,
                Text = "Seçileni Sil",
                BackColor = Color.LightCoral
            };
            btnDelete.Click += BtnDelete_Click;

            // Kontrolleri Ekle
            this.Controls.Add(lstAssignments);
            this.Controls.Add(txtCourseName);
            this.Controls.Add(dtpDueDate);
            this.Controls.Add(txtDescription);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnDelete);
        }

        private void LoadAssignments()
        {
            _isLoading = true; // Yükleme sırasında tetiklenmeyi engelle
            lstAssignments.Items.Clear();
            List<Assignment> list = _service.GetAll();

            foreach (var item in list)
            {
                // Listeye Wrapper nesnesi ekliyoruz
                AssignmentItem wrapper = new AssignmentItem(item);

                // Veritabanındaki duruma (IsCompleted) göre kutucuğu işaretle
                lstAssignments.Items.Add(wrapper, item.IsCompleted);
            }
            _isLoading = false;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (txtCourseName.Text == "Ders Adı" || string.IsNullOrWhiteSpace(txtCourseName.Text))
            {
                MessageBox.Show("Lütfen ders adı giriniz.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string description = txtDescription.Text == "Ödev içeriği / notlar..." ? "" : txtDescription.Text;

            // Service üzerinden ekleme yapıyoruz
            var result = _service.Create(txtCourseName.Text, description, dtpDueDate.Value);

            if (result.Item1) // Başarılıysa
            {
                LoadAssignments();
                // Reset Inputs
                ResetInput(txtCourseName, "Ders Adı");
                ResetInput(txtDescription, "Ödev içeriği / notlar...");
                dtpDueDate.Value = DateTime.Now;
            }
            else
            {
                MessageBox.Show(result.Item2, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lstAssignments.SelectedItem == null)
            {
                MessageBox.Show("Silmek için listeden bir ödevin yazısına tıklayarak seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AssignmentItem selected = lstAssignments.SelectedItem as AssignmentItem;
            if (selected != null)
            {
                if (MessageBox.Show("Bu ödevi silmek istiyor musunuz?", "Sil", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _service.Delete(selected.Data.Id);
                    LoadAssignments();
                }
            }
        }

        // Tik işaretlendiğinde veya kaldırıldığında çalışır
        private void LstAssignments_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_isLoading) return; // Yükleme anındaysa işlem yapma

            AssignmentItem item = lstAssignments.Items[e.Index] as AssignmentItem;
            if (item != null)
            {
                bool isChecked = e.NewValue == CheckState.Checked;
                _service.ToggleStatus(item.Data.Id, isChecked);
            }
        }

        // --- YARDIMCI SINIF VE METOTLAR ---

        // Listbox içinde görünecek nesne yapısı (Ekranda ne yazacağını ayarlar)
        private class AssignmentItem
        {
            public Assignment Data { get; private set; }
            public AssignmentItem(Assignment data) { Data = data; }

            public override string ToString()
            {
                // Görünüm Örneği: "Matematik - Sayfa 10 Çözülecek (25.12.2025)"
                return $"{Data.CourseName} - {Data.Description} ({Data.DueDate:dd.MM.yyyy})";
            }
        }

        private void SetPlaceholder(TextBox txt, string placeholder)
        {
            txt.Text = placeholder;
            txt.ForeColor = Color.Gray;

            txt.GotFocus += (s, e) => {
                if (txt.Text == placeholder) { txt.Text = ""; txt.ForeColor = Color.Black; }
            };

            txt.LostFocus += (s, e) => {
                if (string.IsNullOrWhiteSpace(txt.Text)) { txt.Text = placeholder; txt.ForeColor = Color.Gray; }
            };
        }

        private void ResetInput(TextBox txt, string placeholder)
        {
            txt.Text = placeholder;
            txt.ForeColor = Color.Gray;
        }

        // Form Designer için gerekli boş metot (Otomatik kodlar karışmasın diye)
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(500, 450);
            this.Name = "OdevForm";
            this.ResumeLayout(false);
        }
    }
}