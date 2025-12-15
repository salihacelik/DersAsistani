using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace DersAsistani.Forms
{
    public class AyarlarForm : Form
    {
        public AyarlarForm()
        {
            // Form Ayarları
            this.Text = "Ayarlar";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            InitializeControls();
        }

        private void InitializeControls()
        {
            // Başlık
            Label lblInfo = new Label
            {
                Text = "Ders Asistanı v1.0",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                AutoSize = true,
                Left = 110,
                Top = 30
            };

            Label lblSub = new Label
            {
                Text = "Geliştiriciler:\n Saliha Çelik \n Dilek Ünlü \n Serra Özturhan \n© 2025 Tüm Hakları Saklıdır.",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Left = 115,
                Top = 60
            };

            // Tehlikeli Bölge Grubu
            GroupBox grpDanger = new GroupBox
            {
                Text = "Veri Yönetimi",
                Left = 20,
                Top = 120,
                Width = 340,
                Height = 100,
                Font = new Font("Segoe UI", 9)
            };

            Button btnReset = new Button
            {
                Text = "Tüm Verileri Sıfırla",
                BackColor = Color.IndianRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Left = 70,
                Top = 35,
                Width = 200,
                Height = 40,
                Cursor = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Click += BtnReset_Click;

            grpDanger.Controls.Add(btnReset);

            this.Controls.Add(lblInfo);
            this.Controls.Add(lblSub);
            this.Controls.Add(grpDanger);
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "DİKKAT!\n\nTüm dersler, ödevler, planlar ve notlar KALICI OLARAK silinecek.\nBunu yapmak istediğine emin misin?",
                "Veri Sıfırlama",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                ResetDatabase();
                MessageBox.Show("Uygulama fabrika ayarlarına döndürüldü.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private void ResetDatabase()
        {
            try
            {
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DersAsistani.db");
                string connStr = $"Data Source={dbPath};Version=3;";

                using (var conn = new SQLiteConnection(connStr))
                {
                    conn.Open();
                    // Tüm tabloların içini boşaltıyoruz (Tabloyu silmeden)
                    string[] tables = { "Assignments", "Schedules", "Notes", "Courses" }; // Grades vs varsa ekle

                    foreach (var table in tables)
                    {
                        // Hata almamak için tablo var mı kontrolü yapılabilir ama
                        // basitçe try-catch içinde delete komutu gönderiyoruz.
                        try
                        {
                            using (var cmd = new SQLiteCommand($"DELETE FROM {table}", conn))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                        catch { /* Tablo yoksa devam et */ }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AyarlarForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "AyarlarForm";
            this.Load += new System.EventHandler(this.AyarlarForm_Load);
            this.ResumeLayout(false);

        }

        private void AyarlarForm_Load(object sender, EventArgs e)
        {

        }
    }
}