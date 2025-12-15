using System;
using System.Collections.Generic;
using System.Drawing; // Renkler için gerekli
using System.Windows.Forms;
using DersAsistani.Data;
using DersAsistani.Models;
using DersAsistani.Services;

namespace DersAsistani.Forms
{
    public class DersForm : Form
    {
        // Kontrollerin Tanımlanması
        private ListBox lstCourses;
        private TextBox txtName;
        private TextBox txtInstructor;
        private TextBox txtSchedule;
        private Button btnAdd;
        private Button btnDelete; // Yeni Silme Butonu

        private CourseRepository _repo;
        private CourseService _service;

        public DersForm()
        {
            // Form Ayarları
            this.Text = "Ders Yönetimi";
            this.Width = 450;
            this.Height = 400;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            InitializeCustomControls(); // Kontrol oluşturma işini ayırdık

            _repo = new CourseRepository();
            _service = new CourseService(_repo);

            LoadCourses();
        }

        private void InitializeCustomControls()
        {
            // 1. Liste Kutusu
            lstCourses = new ListBox
            {
                Left = 20,
                Top = 20,
                Width = 390,
                Height = 200
            };

            // 2. Metin Kutuları (Placeholder özelliği aşağıda tanımlandı)
            txtName = new TextBox { Left = 20, Top = 240, Width = 120 };
            txtInstructor = new TextBox { Left = 150, Top = 240, Width = 120 };
            txtSchedule = new TextBox { Left = 280, Top = 240, Width = 130 };

            // Placeholder'ları ayarla
            SetPlaceholder(txtName, "Ders Adı");
            SetPlaceholder(txtInstructor, "Eğitmen");
            SetPlaceholder(txtSchedule, "Gün ve Saat");

            // 3. Ekle Butonu
            btnAdd = new Button
            {
                Left = 20,
                Top = 280,
                Width = 250,
                Height = 40,
                Text = "Ders Ekle",
                BackColor = Color.LightGreen
            };
            btnAdd.Click += BtnAdd_Click;

            // 4. Sil Butonu (YENİ)
            btnDelete = new Button
            {
                Left = 280,
                Top = 280,
                Width = 130,
                Height = 40,
                Text = "Seçileni Sil",
                BackColor = Color.LightCoral
            };
            btnDelete.Click += BtnDelete_Click;

            // Kontrolleri Forma Ekle
            this.Controls.Add(lstCourses);
            this.Controls.Add(txtName);
            this.Controls.Add(txtInstructor);
            this.Controls.Add(txtSchedule);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnDelete);
        }

        private void LoadCourses()
        {
            lstCourses.Items.Clear();
            List<Course> list = _service.GetAll();

            foreach (var c in list)
            {
                // ListBox'a nesnenin kendisini sarmalayan (wrapper) bir yapı ekliyoruz.
                // Böylece hem ekranda güzel görünür hem de silerken ID'sini biliriz.
                lstCourses.Items.Add(new CourseItem(c));
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // Placeholder kontrolü: Eğer kullanıcı hiçbir şey yazmadıysa uyarı ver
            if (txtName.Text == "Ders Adı" || txtInstructor.Text == "Eğitmen" || txtSchedule.Text == "Gün ve Saat")
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = _service.Create(txtName.Text, txtInstructor.Text, txtSchedule.Text);
            bool ok = result.Item1;
            string message = result.Item2;

            if (ok)
            {
                MessageBox.Show(message, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCourses();

                // Kutuları temizle ve placeholder'ları geri getir
                ResetInput(txtName, "Ders Adı");
                ResetInput(txtInstructor, "Eğitmen");
                ResetInput(txtSchedule, "Gün ve Saat");
            }
            else
            {
                MessageBox.Show(message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // SİLME İŞLEMİ
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lstCourses.SelectedItem == null)
            {
                MessageBox.Show("Lütfen silmek için listeden bir ders seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Seçilen öğeyi CourseItem olarak al
            CourseItem selectedItem = lstCourses.SelectedItem as CourseItem;
            if (selectedItem != null)
            {
                DialogResult dialogResult = MessageBox.Show($"'{selectedItem.Course.Name}' dersini silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialogResult == DialogResult.Yes)
                {
                    _service.Delete(selectedItem.Course.Id); // Servis üzerinden sil
                    LoadCourses(); // Listeyi yenile
                }
            }
        }

        // --- YARDIMCI METOTLAR ---

        // ListBox içinde dersin nasıl görüneceğini ayarlayan yardımcı sınıf
        private class CourseItem
        {
            public Course Course { get; private set; }
            public CourseItem(Course course) { Course = course; }

            // ListBox bu metodu çağırarak ekranda ne yazacağını belirler
            public override string ToString()
            {
                return $"{Course.Name} - {Course.Instructor} ({Course.Day} {Course.StartTime:HH:mm})";
            }
        }

        // Placeholder Ayarlayıcı
        private void SetPlaceholder(TextBox txt, string placeholder)
        {
            txt.Text = placeholder;
            txt.ForeColor = Color.Gray;

            txt.GotFocus += (s, e) =>
            {
                if (txt.Text == placeholder)
                {
                    txt.Text = "";
                    txt.ForeColor = Color.Black;
                }
            };

            txt.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = placeholder;
                    txt.ForeColor = Color.Gray;
                }
            };
        }

        // İşlemden sonra kutuyu sıfırlayıp placeholder'ı geri getiren metot
        private void ResetInput(TextBox txt, string placeholder)
        {
            txt.Text = placeholder;
            txt.ForeColor = Color.Gray;
        }

        // Form Designer (Otomatik kodlar karışmasın diye boş bırakıyoruz, yukarıda InitializeCustomControls kullandık)
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(450, 400);
            this.Name = "DersForm";
            this.ResumeLayout(false);
        }
    }
}