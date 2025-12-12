using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DersAsistani.Models;
using DersAsistani.Utils;

namespace DersAsistani.Forms
{
    public class MainForm : Form
    {
        private readonly User _user;

        // Üst bölüm
        private Panel headerPanel;
        private Label lblWelcome;

        // Yan menü
        private Panel navPanel;
        private Button btnNavHome;
        private Button btnNavCourses;
        private Button btnNavAssignments;
        private Button btnNavPlans;
        private Button btnNavNotes;
        private Button btnNavSettings;

        // Sol: Takvim ve etkinlikler
        private Panel leftPanel;
        private MonthCalendar monthCalendar;
        private ListBox lstEvents;
        private Label lblSelectedDate;

        // Sağ: Not defteri
        private Panel rightPanel;
        private Label lblNotesTitle;
        private TextBox txtNotes;
        private Button btnSaveNotes;
        private Button btnClearNotes;

        // Alt: Ders programı (örnek)
        private Panel bottomPanel;
        private DataGridView dgvSchedule;
        private Label lblScheduleTitle;

        private readonly string _notesPath;

        public MainForm(User user)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _notesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"notes_{SanitizeFileName(_user.Username)}.txt");

            InitializeForm();
            InitializeControls();
            LayoutControls();
            AttachEvents();

            LoadNotes();
            LoadSampleSchedule();
        }

        private void InitializeForm()
        {
            this.Text = "Ders Asistanı - Ana Sayfa";
            this.Size = new Size(980, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(240, 242, 245);
            UIHelper.ApplyFormTheme(this);
        }

        private void InitializeControls()
        {
            // Header
            headerPanel = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.White };
            lblWelcome = new Label
            {
                Text = $"Hoş geldiniz, {_user.FullName ?? _user.Username}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(20, 22)
            };
            headerPanel.Controls.Add(lblWelcome);

            // Nav panel (yan menü) - MAVİ renk
            navPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 96,
                BackColor = Color.FromArgb(52, 152, 219), // mavi ton
                Padding = new Padding(8)
            };

            btnNavHome = CreateNavButton("Ana Sayfa");
            btnNavCourses = CreateNavButton("Dersler");
            btnNavAssignments = CreateNavButton("Ödevler");
            btnNavPlans = CreateNavButton("Planlar");
            btnNavNotes = CreateNavButton("Notlar");
            btnNavSettings = CreateNavButton("Ayarlar");

            // Left panel (takvim + etkinlikler)
            leftPanel = new Panel { BackColor = Color.White, Width = 360, Dock = DockStyle.Left, Padding = new Padding(12,12,12,24) }; // alt padding ile boşluk verildi
            monthCalendar = new MonthCalendar
            {
                MaxSelectionCount = 1,
                ShowToday = true,
                ShowTodayCircle = true,
                CalendarDimensions = new Size(1, 1),
                FirstDayOfWeek = Day.Monday,
                BackColor = Color.White,
                Location = new Point(12, 8) // padding ile uyumlu konumlandırma
            };
            lblSelectedDate = new Label
            {
                Text = "Seçili tarih: -",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true
            };
            lstEvents = new ListBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(248, 249, 250),
                Font = new Font("Segoe UI", 10),
                Height = 160
            };

            // Right panel (not defteri)
            rightPanel = new Panel { BackColor = Color.White, Dock = DockStyle.Fill, Padding = new Padding(12) };
            lblNotesTitle = new Label
            {
                Text = "Not Defteri (madde madde yazın)",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true
            };
            txtNotes = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Top,
                Height = 320
            };
            btnSaveNotes = new Button
            {
                Text = "Kaydet",
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 120,
                Height = 40,
                Cursor = Cursors.Hand
            };
            btnSaveNotes.FlatAppearance.BorderSize = 0;

            btnClearNotes = new Button
            {
                Text = "Temizle",
                BackColor = Color.White,
                ForeColor = Color.FromArgb(52, 152, 219),
                FlatStyle = FlatStyle.Flat,
                Width = 120,
                Height = 40,
                Cursor = Cursors.Hand
            };
            btnClearNotes.FlatAppearance.BorderSize = 1;
            btnClearNotes.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);

            // Bottom panel (ders programı)
            bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 240, BackColor = Color.White, Padding = new Padding(12) };
            lblScheduleTitle = new Label
            {
                Text = "Ders Programı (örnek)",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true
            };
            dgvSchedule = new DataGridView
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill
            };
        }

        private Button CreateNavButton(string text)
        {
            var b = new Button
            {
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 56,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White, // beyaz yazı, mavi arka planda okunur
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            // hover style - biraz koyu mavi
            b.MouseEnter += (s, e) => b.BackColor = Color.FromArgb(41, 128, 185);
            b.MouseLeave += (s, e) => b.BackColor = Color.Transparent;
            navPanel?.Controls.Add(b);
            return b;
        }

        private void LayoutControls()
        {
            // navPanel: ekle butonları (ters sırada ekle ki görünüm doğru olsun)
            navPanel.Controls.Clear();
            navPanel.Controls.Add(btnNavSettings);
            navPanel.Controls.Add(btnNavNotes);
            navPanel.Controls.Add(btnNavPlans);
            navPanel.Controls.Add(btnNavAssignments);
            navPanel.Controls.Add(btnNavCourses);
            navPanel.Controls.Add(btnNavHome);

            // leftPanel layout
            leftPanel.Controls.Add(monthCalendar);
            // lblSelectedDate ve etkinlikleri calendar'ın altına yerleştir
            lblSelectedDate.Location = new Point(12, monthCalendar.Bottom + 10);
            leftPanel.Controls.Add(lblSelectedDate);

            var lblEvents = new Label
            {
                Text = "Bugün / Yaklaşan Etkinlikler",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true
            };
            lblEvents.Location = new Point(12, lblSelectedDate.Bottom + 8);
            lstEvents.Location = new Point(12, lblEvents.Bottom + 6);
            lstEvents.Width = leftPanel.Width - 36;
            leftPanel.Controls.Add(lblEvents);
            leftPanel.Controls.Add(lstEvents);

            // rightPanel layout
            lblNotesTitle.Location = new Point(12, 12);
            txtNotes.Location = new Point(12, lblNotesTitle.Bottom + 8);
            // buttons panel under notes
            FlowLayoutPanel btnPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Bottom,
                Height = 56,
                Padding = new Padding(12),
                BackColor = Color.Transparent
            };
            btnPanel.Controls.Add(btnSaveNotes);
            btnPanel.Controls.Add(new Label { Width = 8 });
            btnPanel.Controls.Add(btnClearNotes);

            rightPanel.Controls.Add(btnPanel);
            rightPanel.Controls.Add(txtNotes);
            rightPanel.Controls.Add(lblNotesTitle);

            // bottomPanel layout
            lblScheduleTitle.Location = new Point(12, 12);
            dgvSchedule.Location = new Point(12, lblScheduleTitle.Bottom + 8);
            dgvSchedule.Height = bottomPanel.Height - lblScheduleTitle.Height - 36;
            bottomPanel.Controls.Add(lblScheduleTitle);
            bottomPanel.Controls.Add(dgvSchedule);

            // Ekleme sırası: nav, left, right, spacer (opsiyonel) ve bottom, header
            // Spacer: takvim ile program arasına boşluk koymak için
            var spacer = new Panel { Dock = DockStyle.Bottom, Height = 12, BackColor = Color.Transparent };

            // Add to form - dikkat: docking sırası önemli
            this.Controls.Add(rightPanel);   // Fill
            this.Controls.Add(leftPanel);    // Left (rightPanel will fill the remaining)
            this.Controls.Add(navPanel);     // Left-most
            this.Controls.Add(spacer);       // Spacer above bottomPanel
            this.Controls.Add(bottomPanel);  // Bottom
            this.Controls.Add(headerPanel);  // Top
        }

        private void AttachEvents()
        {
            monthCalendar.DateChanged += MonthCalendar_DateChanged;
            btnSaveNotes.Click += BtnSaveNotes_Click;
            btnClearNotes.Click += BtnClearNotes_Click;
            this.FormClosed += MainForm_FormClosed;

            // Nav button events
            btnNavHome.Click += (s, e) => ShowHome();
            btnNavCourses.Click += (s, e) => OpenCourses();
            btnNavAssignments.Click += (s, e) => OpenAssignments();
            btnNavPlans.Click += (s, e) => OpenPlans();
            btnNavNotes.Click += (s, e) => FocusNotes();
            btnNavSettings.Click += (s, e) => ShowSettings();
        }

        private void ShowHome()
        {
            // Ana gösterim: takvim + not defteri + ders programı visible
            leftPanel.Visible = true;
            rightPanel.Visible = true;
            bottomPanel.Visible = true;
        }

        private void OpenCourses()
        {
            using (var f = new DersForm())
            {
                f.ShowDialog(this);
            }
        }

        private void OpenAssignments()
        {
            using (var f = new OdevForm())
            {
                f.ShowDialog(this);
            }
        }

        private void OpenPlans()
        {
            using (var f = new PlanForm())
            {
                f.ShowDialog(this);
            }
        }

        private void FocusNotes()
        {
            ShowHome();
            txtNotes.Focus();
        }

        private void ShowSettings()
        {
            MessageBox.Show("Ayarlar yakında eklenecek.", "Ayarlar", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MonthCalendar_DateChanged(object sender, DateRangeEventArgs e)
        {
            var date = e.Start;
            lblSelectedDate.Text = $"Seçili tarih: {date:yyyy-MM-dd}";
            lstEvents.Items.Clear();
            if (date.Date == DateTime.Today)
            {
                lstEvents.Items.Add("09:00 - Matematik (Sınıf 101)");
                lstEvents.Items.Add("13:00 - Programlama (Sınıf 203)");
            }
            else
            {
                lstEvents.Items.Add("Bugün için etkinlik yok.");
            }
        }

        private void BtnSaveNotes_Click(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllText(_notesPath, txtNotes.Text);
                MessageBox.Show("Notlar kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Notlar kaydedilirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearNotes_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Notları temizlemek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                txtNotes.Clear();
            }
        }

        private void LoadNotes()
        {
            try
            {
                if (File.Exists(_notesPath))
                {
                    txtNotes.Text = File.ReadAllText(_notesPath);
                }
                else
                {
                    txtNotes.Text = "- Ders notları için maddeler yazabilirsiniz.\r\n- Ödev tarihlerini buraya not alın.\r\n- Kısa hatırlatmalar...";
                }
            }
            catch
            {
                // silent
            }
        }

        private void LoadSampleSchedule()
        {
            dgvSchedule.Columns.Clear();
            dgvSchedule.Columns.Add("Day", "Gün");
            dgvSchedule.Columns.Add("Time", "Saat");
            dgvSchedule.Columns.Add("Course", "Ders");
            dgvSchedule.Columns.Add("Room", "Sınıf / Yer");

            var sample = new List<Tuple<string, string, string, string>>()
            {
                Tuple.Create("Pazartesi", "09:00 - 10:30", "Matematik", "101"),
                Tuple.Create("Pazartesi", "11:00 - 12:30", "Fizik", "102"),
                Tuple.Create("Salı", "13:00 - 14:30", "Programlama", "203"),
                Tuple.Create("Çarşamba", "10:45 - 12:15", "Kimya", "105"),
                Tuple.Create("Perşembe", "14:45 - 16:15", "Tarih", "110"),
                Tuple.Create("Cuma", "09:00 - 10:30", "Beden Eğitimi", "Spor Salonu")
            };

            dgvSchedule.Rows.Clear();
            foreach (var r in sample)
            {
                dgvSchedule.Rows.Add(r.Item1, r.Item2, r.Item3, r.Item4);
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // otomatik kaydetme veya temizleme burada olabilir
            }
            catch { }
        }

        private string SanitizeFileName(string input)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                input = input.Replace(c.ToString(), "_");
            }
            return input;
        }
    }
}