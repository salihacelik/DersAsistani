using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DersAsistani.Models;
using DersAsistani.Services;
using DersAsistani.Utils;

namespace DersAsistani.Forms
{
    public partial class MainForm : Form
    {
        private readonly User _user;

        // Services for the Dashboard
        private AssignmentService _assignmentService;
        private ScheduleService _scheduleService;

        // --- UI Controls ---
        // Title Bar
        private Panel titleBar;
        private Label lblWelcome;
        private Button btnMinimize;
        private Button btnMaximize;
        private Button btnClose;
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        // Navigation Menu
        private Panel navPanel;
        private Button btnNavHome;
        private Button btnNavCourses;
        private Button btnNavAssignments;
        private Button btnNavPlans;
        private Button btnNavNotes;
        private Button btnNavSettings;
        private Button btnNavLogout; // --- YENİ EKLENDİ: Çıkış Butonu Tanımı ---

        // Left Panel (Calendar + Clock)
        private Panel leftPanel;
        private MonthCalendar monthCalendar;
        private Label lblClock;
        private Label lblSelectedDate;

        // Right Panel (Events / Dashboard)
        private Panel eventsPanel;
        private Label lblEventsTitle;
        private ListBox lstEvents;

        // Meetings Panel (Top Right)
        private Panel meetingsPanel;
        private Label lblMeetingsTitle;
        private ListBox lstMeetings;

        // Clock Timer
        private Timer clockTimer;

        // Constructor
        public MainForm(User user)
        {
            // If user is null, create a dummy user to prevent crash during testing
            _user = user ?? new User { FullName = "Kullanıcı", Username = "User" };

            // Initialize Services
            _assignmentService = new AssignmentService();
            _scheduleService = new ScheduleService();

            // 1. Setup Form Properties
            InitializeFormProperties();

            // 2. Create and Layout Controls (Custom UI)
            InitializeCustomControls();
            LayoutControls();

            // 3. Attach Event Handlers
            AttachEvents();

            // 4. Start Clock
            StartClock();

            // 5. Initial Data Load for Dashboard
            RefreshDashboard();
        }

        // Default constructor for Designer support
        public MainForm() : this(null) { }

        private void InitializeFormProperties()
        {
            this.Text = "Ders Asistanı - Ana Sayfa";
            this.Size = new Size(980, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(240, 242, 245);
        }

        private void InitializeCustomControls()
        {
            // Title Bar
            titleBar = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Color.White };
            lblWelcome = new Label
            {
                Text = $"Hoş geldiniz, {_user.FullName ?? _user.Username}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(16, 12)
            };
            btnMinimize = CreateTitleButton("—");
            btnMaximize = CreateTitleButton("⬜");
            btnClose = CreateTitleButton("✕");

            // Nav Panel
            navPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 96,
                BackColor = Color.FromArgb(52, 152, 219),
                Padding = new Padding(8)
            };

            btnNavHome = CreateNavButton("Ana Sayfa");
            btnNavCourses = CreateNavButton("Dersler");
            btnNavAssignments = CreateNavButton("Ödevler");
            btnNavPlans = CreateNavButton("Planlar");
            btnNavNotes = CreateNavButton("Notlar");
            btnNavSettings = CreateNavButton("Ayarlar");

            // --- YENİ EKLENDİ: Çıkış Butonunu Oluşturma ---
            btnNavLogout = CreateNavButton("Çıkış Yap");
            // Çıkış butonunu biraz farklı renklendirelim (Kırmızımsı hover efekti)
            btnNavLogout.MouseEnter += (s, e) => btnNavLogout.BackColor = Color.FromArgb(192, 57, 43);

            // Left Panel (Calendar)
            leftPanel = new Panel { BackColor = Color.White, Width = 300, Dock = DockStyle.Left, Padding = new Padding(12) };
            monthCalendar = new MonthCalendar
            {
                MaxSelectionCount = 1,
                ShowToday = true,
                ShowTodayCircle = true,
                FirstDayOfWeek = Day.Monday,
                BackColor = Color.White,
                Location = new Point(12, 8)
            };
            lblClock = new Label
            {
                Text = DateTime.Now.ToString("HH:mm:ss"),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(12, 180)
            };
            lblSelectedDate = new Label
            {
                Text = $"Seçili tarih: {DateTime.Now:yyyy-MM-dd}",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(12, 210)
            };

            // Meetings Panel
            meetingsPanel = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = Color.FromArgb(250, 250, 250), Padding = new Padding(8) };
            lblMeetingsTitle = new Label
            {
                Text = "Toplantılar / Görüşmeler",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(6, 6)
            };
            lstMeetings = new ListBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(248, 249, 250),
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill
            };
            lstMeetings.Items.Add("10:30 - Proje Toplantısı (Zoom)");

            // Events Panel
            eventsPanel = new Panel { BackColor = Color.White, Dock = DockStyle.Fill, Padding = new Padding(12) };
            lblEventsTitle = new Label
            {
                Text = "Yaklaşan Etkinlikler (Ödevler & Planlar)",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Dock = DockStyle.Top
            };
            lstEvents = new ListBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(248, 249, 250),
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Fill
            };
        }

        private void LayoutControls()
        {
            // Title Bar Layout
            titleBar.Controls.Add(lblWelcome);
            titleBar.Controls.AddRange(new Control[] { btnMinimize, btnMaximize, btnClose });
            PositionTitleButtons();

            // Nav Panel Layout (Dock.Top mantığıyla ters sıra: En son eklenen en üstte olur)
            navPanel.Controls.Clear();

            // --- YENİ EKLENDİ: Çıkış butonunu buraya ekliyoruz ki en altta görünsün (Ayarlar'ın altında) ---
            navPanel.Controls.Add(btnNavLogout);

            navPanel.Controls.Add(btnNavSettings);
            navPanel.Controls.Add(btnNavNotes);
            navPanel.Controls.Add(btnNavPlans);
            navPanel.Controls.Add(btnNavAssignments);
            navPanel.Controls.Add(btnNavCourses);
            navPanel.Controls.Add(btnNavHome); // En son eklendiği için EN ÜSTTE görünür

            // Left Panel Layout
            leftPanel.Controls.Add(monthCalendar);
            leftPanel.Controls.Add(lblClock);
            leftPanel.Controls.Add(lblSelectedDate);

            // Meetings Panel Layout
            meetingsPanel.Controls.Add(lstMeetings);
            meetingsPanel.Controls.Add(lblMeetingsTitle);

            // Events Panel Layout
            eventsPanel.Controls.Add(lstEvents);
            eventsPanel.Controls.Add(lblEventsTitle);
            eventsPanel.Controls.Add(meetingsPanel);

            // Main Form Layout
            this.Controls.Add(eventsPanel);
            this.Controls.Add(leftPanel);
            this.Controls.Add(navPanel);
            this.Controls.Add(titleBar);
        }

        private void AttachEvents()
        {
            // Window Dragging
            titleBar.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { dragging = true; dragCursorPoint = Cursor.Position; dragFormPoint = this.Location; } };
            titleBar.MouseMove += (s, e) => { if (dragging) { Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint)); this.Location = Point.Add(dragFormPoint, new Size(dif)); } };
            titleBar.MouseUp += (s, e) => { dragging = false; };

            // Window Controls
            btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            btnMaximize.Click += (s, e) => this.WindowState = this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
            btnClose.Click += (s, e) => Application.Exit();

            // Calendar
            monthCalendar.DateChanged += (s, e) => { lblSelectedDate.Text = $"Seçili tarih: {e.Start:yyyy-MM-dd}"; };

            // Navigation Buttons
            btnNavHome.Click += (s, e) => RefreshDashboard();
            btnNavCourses.Click += (s, e) => { new DersForm().ShowDialog(); RefreshDashboard(); };
            btnNavAssignments.Click += (s, e) => { new OdevForm().ShowDialog(); RefreshDashboard(); };
            btnNavPlans.Click += (s, e) => { new PlanForm().ShowDialog(); RefreshDashboard(); };
            btnNavNotes.Click += (s, e) => { new NotlarForm().ShowDialog(); };

            // Ayarlar Butonu
            btnNavSettings.Click += (s, e) =>
            {
                AyarlarForm form = new AyarlarForm();
                form.ShowDialog();
                RefreshDashboard();
            };

            // --- YENİ EKLENDİ: Çıkış Butonu İşlevi ---
            btnNavLogout.Click += (s, e) =>
            {
                DialogResult cvp = MessageBox.Show("Hesabınızdan çıkış yapmak istediğinize emin misiniz?",
                                                   "Çıkış Yap",
                                                   MessageBoxButtons.YesNo,
                                                   MessageBoxIcon.Question);

                if (cvp == DialogResult.Yes)
                {
                    // LoginForm'u (Giriş Ekranı) aç
                    // Not: Projende giriş formunun adı 'LoginForm' değilse burayı düzeltmelisin (örn: FrmGiris)
                    LoginForm girisEkrani = new LoginForm();
                    girisEkrani.Show();

                    // Ana formu kapat
                    this.Close();
                }
            };

            // Form Resize Event
            this.Resize += (s, e) => PositionTitleButtons();
            titleBar.Resize += (s, e) => PositionTitleButtons();
        }

        // --- DASHBOARD LOGIC (Populates lstEvents) ---
        private void RefreshDashboard()
        {
            if (lstEvents == null) return;
            lstEvents.Items.Clear();

            try
            {
                var assignments = _assignmentService.GetAll();
                var schedules = _scheduleService.GetAll();
                var allEvents = new List<DashboardItem>();

                // Add Assignments
                foreach (var a in assignments)
                {
                    if (!a.IsCompleted && a.DueDate.Date >= DateTime.Now.Date)
                    {
                        allEvents.Add(new DashboardItem
                        {
                            SortDate = a.DueDate,
                            DisplayText = $"[ÖDEV] {a.CourseName} - {a.Description} ({a.DueDate:HH:mm})"
                        });
                    }
                }

                // Add Plans
                foreach (var s in schedules)
                {
                    if (s.Date.Date >= DateTime.Now.Date)
                    {
                        allEvents.Add(new DashboardItem
                        {
                            SortDate = s.Date,
                            DisplayText = $"[{s.Category.ToUpper()}] {s.Title} ({s.Date:HH:mm})"
                        });
                    }
                }

                // Sort by Date
                allEvents.Sort((x, y) => x.SortDate.CompareTo(y.SortDate));

                // Add to ListBox
                foreach (var item in allEvents)
                {
                    string dateStr = item.SortDate.ToString("dd.MM");
                    lstEvents.Items.Add($"{dateStr} - {item.DisplayText}");
                }

                if (lstEvents.Items.Count == 0)
                {
                    lstEvents.Items.Add("Yaklaşan etkinlik yok. Keyfine bak!");
                }
            }
            catch (Exception ex)
            {
                lstEvents.Items.Add("Veri yüklenemedi.");
                Console.WriteLine(ex.Message);
            }
        }

        // --- Helper Methods ---
        private void StartClock()
        {
            clockTimer = new Timer { Interval = 1000 };
            clockTimer.Tick += (s, e) => lblClock.Text = DateTime.Now.ToString("HH:mm:ss");
            clockTimer.Start();
        }

        private Button CreateTitleButton(string text)
        {
            var b = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Size = new Size(36, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(52, 73, 94),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            b.FlatAppearance.BorderSize = 0;
            b.MouseEnter += (s, e) => b.BackColor = Color.FromArgb(240, 240, 240);
            b.MouseLeave += (s, e) => b.BackColor = Color.Transparent;
            return b;
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
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.MouseEnter += (s, e) => b.BackColor = Color.FromArgb(41, 128, 185);
            b.MouseLeave += (s, e) => b.BackColor = Color.Transparent;
            return b;
        }

        private void PositionTitleButtons()
        {
            const int margin = 8;
            int top = 9;
            if (titleBar == null || btnClose == null) return;
            btnClose.Location = new Point(titleBar.ClientSize.Width - margin - btnClose.Width, top);
            btnMaximize.Location = new Point(btnClose.Left - 4 - btnMaximize.Width, top);
            btnMinimize.Location = new Point(btnMaximize.Left - 4 - btnMinimize.Width, top);
            btnClose.BringToFront();
            btnMaximize.BringToFront();
            btnMinimize.BringToFront();
        }

        private class DashboardItem
        {
            public DateTime SortDate { get; set; }
            public string DisplayText { get; set; }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "MainForm";
            // this.Load += new System.EventHandler(this.MainForm_Load); // Bu event yoksa hata verebilir, şimdilik kapattım.
            this.ResumeLayout(false);
        }
    }
}