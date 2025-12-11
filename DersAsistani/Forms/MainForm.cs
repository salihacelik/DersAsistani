// Forms/MainForm.cs
using System;
using System.Drawing;
using System.Windows.Forms;
using DersAsistani.Models;
using System.Drawing;
using DersAsistani.Utils;

namespace DersAsistani.Forms
{
    public class MainForm : Form
    {
        private readonly User _user;
        private MonthCalendar calendar = new MonthCalendar { Left = 200, Top = 60 };
        private Panel sidebar = new Panel { Left = 0, Top = 0, Width = 180, Dock = DockStyle.Left, BackColor = Color.FromArgb(35, 40, 45) };
        private Label lblUser = new Label { Left = 200, Top = 20, AutoSize = true };

        public MainForm(User user)
        {
            _user = user;
            this.Text = "Ders Asistanı - Ana Menü";
            this.Width = 900;
            this.Height = 600;

            lblUser.Text = "Hoş geldin, " + _user.FullName;
            this.Controls.Add(lblUser);
            this.Controls.Add(calendar);
            this.Controls.Add(sidebar);

            // Sidebar butonları
            var btn = new Button { /* konum, boyut, metin */ };
            btn.ForeColor = Color.White;
            btn.BackColor = Color.Transparent;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            UIHelper.StyleButton(btn);
            AddSidebarButton("Dersler", delegate { new DersForm().ShowDialog(); });
            AddSidebarButton("Ödevler", delegate { new OdevForm().ShowDialog(); });
            AddSidebarButton("Planlarım", delegate { MessageBox.Show("Planlarım modülü yakında."); });
            AddSidebarButton("Sınavlar", delegate { MessageBox.Show("Sınavlar modülü yakında."); });
            AddSidebarButton("Çalışma Odası", delegate { new StudyRoomForm().ShowDialog(); });
            AddSidebarButton("Raporlar", delegate { new RaporForm().ShowDialog(); });
            AddSidebarButton("Çıkış", delegate
            {
                var confirm = MessageBox.Show("Çıkmak istiyor musun?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    this.Close(); // Form1’e geri dönüş
                }
            });

            calendar.DateChanged += Calendar_DateChanged;
        }

        private void Calendar_DateChanged(object sender, DateRangeEventArgs e)
        {
            var date = e.Start.ToString("yyyy-MM-dd");
            MessageBox.Show("Seçili gün: " + date, "Takvim", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AddSidebarButton(string text, Action onClick)
        {
            var btn = new Button
            {
                Text = text,
                Height = 40,
                Dock = DockStyle.Top,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += delegate { onClick(); };
            sidebar.Controls.Add(btn);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            UIHelper.ApplyFormTheme(this);
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}