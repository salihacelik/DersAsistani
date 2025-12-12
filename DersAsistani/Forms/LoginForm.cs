using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading.Tasks;
using DersAsistani.Data;
using DersAsistani.Models;
using DersAsistani.Services;

namespace DersAsistani.Forms
{
    public class LoginForm : Form
    {
        private Panel mainPanel;
        private Panel loginPanel;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnShowPassword;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblUsername;
        private Label lblPassword;
        private PictureBox picLogo;
        private LinkLabel lnkRegister;
        private ProgressBar progressBar;
        private Label lblError;

        public User LoggedInUser { get; private set; }

        public LoginForm()
        {
            InitializeForm();
            InitializeControls();
            SetupLayout();
            AttachEvents();
        }

        private void InitializeForm()
        {
            this.Text = "Ders Asistanı - Giriş Yap";
            this.Size = new Size(450, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Padding = new Padding(0);
            this.MinimumSize = new Size(450, 650);
            this.MaximumSize = new Size(450, 650);
        }

        private void InitializeControls()
        {
            // Ana panel (gradient arka plan için)
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // Gradient arka plan için Paint event
            mainPanel.Paint += (s, e) =>
            {
                using (System.Drawing.Drawing2D.LinearGradientBrush brush = 
                    new System.Drawing.Drawing2D.LinearGradientBrush(
                        mainPanel.ClientRectangle,
                        Color.FromArgb(240, 242, 245),
                        Color.FromArgb(220, 230, 240),
                        System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, mainPanel.ClientRectangle);
                }
            };

            // Giriş paneli (kart görünümü)
            loginPanel = new Panel
            {
                Size = new Size(400, 580),
                BackColor = Color.White,
                Location = new Point(25, 35)
            };

            // Profil resmi (boş, yuvarlak) - Logo yerine
            picLogo = new PictureBox
            {
                Size = new Size(100, 100),
                Location = new Point(150, 20),
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            DrawProfilePicture(picLogo);

            // Başlık - En üstte ortada
            lblTitle = new Label
            {
                Text = "Ders Asistanı",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(0, 130)
            };
            // Ortalama işlemini Load event'inde yap
            this.Load += (s, e) =>
            {
                lblTitle.Left = (loginPanel.Width - lblTitle.Width) / 2;
            };

            // Alt başlık
            lblSubtitle = new Label
            {
                Text = "Hesabınıza giriş yapın",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(0, 170)
            };
            this.Load += (s, e) =>
            {
                lblSubtitle.Left = (loginPanel.Width - lblSubtitle.Width) / 2;
            };

            // Kullanıcı adı etiketi
            lblUsername = new Label
            {
                Text = "Kullanıcı Adı",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(50, 230)
            };

            // Kullanıcı adı textbox
            txtUsername = new TextBox
            {
                Location = new Point(50, 255),
                Size = new Size(300, 40),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250),
                ForeColor = Color.FromArgb(52, 73, 94),
                Padding = new Padding(10, 0, 10, 0)
            };
            txtUsername.Enter += (s, e) => { txtUsername.BackColor = Color.White; };
            txtUsername.Leave += (s, e) => { txtUsername.BackColor = Color.FromArgb(248, 249, 250); };

            // Şifre etiketi
            lblPassword = new Label
            {
                Text = "Şifre",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(50, 320)
            };

            // Şifre paneli (textbox + göster butonu için)
            Panel passwordPanel = new Panel
            {
                Location = new Point(50, 345),
                Size = new Size(300, 40),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Şifre textbox
            txtPassword = new TextBox
            {
                Location = new Point(0, 0),
                Size = new Size(260, 40),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(248, 249, 250), // Transparent yerine panel rengi
                ForeColor = Color.FromArgb(52, 73, 94),
                UseSystemPasswordChar = true,
                Dock = DockStyle.Left,
                Padding = new Padding(10, 0, 0, 0)
            };
            txtPassword.Enter += (s, e) => 
            { 
                passwordPanel.BackColor = Color.White;
                txtPassword.BackColor = Color.White; // TextBox rengini de güncelle
            };
            txtPassword.Leave += (s, e) => 
            { 
                passwordPanel.BackColor = Color.FromArgb(248, 249, 250);
                txtPassword.BackColor = Color.FromArgb(248, 249, 250); // TextBox rengini de güncelle
            };

            // Şifre göster/gizle butonu
            btnShowPassword = new Button
            {
                Text = "👁",
                Size = new Size(40, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(127, 140, 141),
                Font = new Font("Segoe UI", 14),
                Dock = DockStyle.Right,
                Cursor = Cursors.Hand
            };
            btnShowPassword.FlatAppearance.BorderSize = 0;
            btnShowPassword.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
            btnShowPassword.Click += BtnShowPassword_Click;

            passwordPanel.Controls.Add(txtPassword);
            passwordPanel.Controls.Add(btnShowPassword);

            // Hata mesajı label
            lblError = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(231, 76, 60),
                AutoSize = true,
                Location = new Point(50, 400),
                Size = new Size(300, 20),
                Visible = false
            };

            // Giriş butonu
            btnLogin = new Button
            {
                Text = "Giriş Yap",
                Location = new Point(50, 440),
                Size = new Size(300, 45),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(52, 152, 219),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            btnLogin.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 102, 148);
            // Event'i burada bağla
            btnLogin.Click += BtnLogin_Click;

            // Progress bar (loading için)
            progressBar = new ProgressBar
            {
                Location = new Point(50, 440),
                Size = new Size(300, 45),
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Visible = false
            };

            // Kayıt ol linki
            lnkRegister = new LinkLabel
            {
                Text = "Hesabınız yok mu? Kayıt olun",
                Location = new Point(0, 510),
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                LinkColor = Color.FromArgb(52, 152, 219),
                ActiveLinkColor = Color.FromArgb(41, 128, 185),
                VisitedLinkColor = Color.FromArgb(52, 152, 219),
                Cursor = Cursors.Hand
            };
            this.Load += (s, e) =>
            {
                lnkRegister.Left = (loginPanel.Width - lnkRegister.Width) / 2;
            };

            // Kapatma butonu (sağ üst köşe)
            Button btnClose = new Button
            {
                Text = "✕",
                Size = new Size(30, 30),
                Location = new Point(420, 5),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(127, 140, 141),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 76, 60);
            // MouseOverForeColor özelliği yok, event kullanıyoruz
            btnClose.MouseEnter += (s, e) => { btnClose.ForeColor = Color.White; };
            btnClose.MouseLeave += (s, e) => { btnClose.ForeColor = Color.FromArgb(127, 140, 141); };
            btnClose.Click += (s, e) => this.Close();

            // Kontrolleri panele ekle
            loginPanel.Controls.AddRange(new Control[]
            {
                picLogo, lblTitle, lblSubtitle,
                lblUsername, txtUsername,
                lblPassword, passwordPanel,
                lblError, btnLogin, progressBar,
                lnkRegister
            });

            mainPanel.Controls.Add(loginPanel);
            mainPanel.Controls.Add(btnClose);
            this.Controls.Add(mainPanel);
        }

        private void DrawProfilePicture(PictureBox pic)
        {
            Bitmap bmp = new Bitmap(100, 100);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Yuvarlak arka plan (açık gri)
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(236, 240, 241)))
                {
                    g.FillEllipse(brush, 0, 0, 100, 100);
                }
                
                // Yuvarlak kenarlık
                using (Pen pen = new Pen(Color.FromArgb(189, 195, 199), 2))
                {
                    g.DrawEllipse(pen, 1, 1, 98, 98);
                }
                
                // Profil ikonu (basit kullanıcı ikonu)
                using (SolidBrush iconBrush = new SolidBrush(Color.FromArgb(149, 165, 166)))
                {
                    // Kafa (yuvarlak)
                    g.FillEllipse(iconBrush, 30, 25, 40, 40);
                    
                    // Vücut (yuvarlak alt kısım)
                    g.FillEllipse(iconBrush, 20, 60, 60, 40);
                }
            }
            pic.Image = bmp;
        }

        private void SetupLayout()
        {
            // Panel gölge efekti için
            loginPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, loginPanel.ClientRectangle,
                    Color.FromArgb(220, 220, 220), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(220, 220, 220), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(220, 220, 220), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(220, 220, 220), 1, ButtonBorderStyle.Solid);
            };
        }

        private void AttachEvents()
        {
            // btnLogin.Click burada değil, buton oluşturulurken bağlandı
            lnkRegister.LinkClicked += LnkRegister_LinkClicked;
            
            // Enter tuşu ile giriş
            txtPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    BtnLogin_Click(btnLogin, EventArgs.Empty);
                }
            };
            
            txtUsername.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPassword.Focus();
                }
            };
        }

        private void BtnShowPassword_Click(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            btnShowPassword.Text = txtPassword.UseSystemPasswordChar ? "👁" : "👁‍🗨";
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // Validasyon
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ShowError("Lütfen kullanıcı adınızı girin.");
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowError("Lütfen şifrenizi girin.");
                txtPassword.Focus();
                return;
            }

            // Loading göster
            ShowLoading(true);

            // Asenkron işlemi başlat
            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    var repo = new UserRepository();
                    var auth = new AuthService(repo);
                    var result = auth.Login(txtUsername.Text, txtPassword.Text);

                    // UI thread'ine geri dön - güvenli şekilde
                    if (this.IsDisposed || this.Disposing)
                        return;

                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        {
                            if (this.IsDisposed) return;
                            
                            ShowLoading(false);

                            bool ok = result.Item1;
                            string message = result.Item2;
                            User user = result.Item3;

                            if (!ok)
                            {
                                ShowError(message);
                                txtPassword.Clear();
                                txtPassword.Focus();
                                return;
                            }

                            LoggedInUser = user;
                            this.DialogResult = DialogResult.OK;
                        }));
                    }
                    else
                    {
                        ShowLoading(false);

                        bool ok = result.Item1;
                        string message = result.Item2;
                        User user = result.Item3;

                        if (!ok)
                        {
                            ShowError(message);
                            txtPassword.Clear();
                            txtPassword.Focus();
                            return;
                        }

                        LoggedInUser = user;
                        this.DialogResult = DialogResult.OK;
                    }
                }
                catch (Exception ex)
                {
                    if (this.IsDisposed || this.Disposing)
                        return;

                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        {
                            if (this.IsDisposed) return;
                            ShowLoading(false);
                            ShowError("Giriş sırasında bir hata oluştu: " + ex.Message);
                        }));
                    }
                    else
                    {
                        ShowLoading(false);
                        ShowError("Giriş sırasında bir hata oluştu: " + ex.Message);
                    }
                }
            });
        }

        private void ProcessLoginResult(Tuple<bool, string, User> result)
        {
            if (result == null)
            {
                ShowError("Giriş işlemi başarısız oldu.");
                return;
            }

            bool ok = result.Item1;
            string message = result.Item2;
            User user = result.Item3;

            if (!ok)
            {
                ShowError(message);
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            LoggedInUser = user;
            this.DialogResult = DialogResult.OK;
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
            
            // 5 saniye sonra gizle
            Timer timer = new Timer { Interval = 5000 };
            timer.Tick += (s, e) =>
            {
                lblError.Visible = false;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        private void ShowLoading(bool show)
        {
            btnLogin.Visible = !show;
            progressBar.Visible = show;
            btnLogin.Enabled = !show;
            txtUsername.Enabled = !show;
            txtPassword.Enabled = !show;
        }

        private void LnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Form hareket ettirme (FormBorderStyle.None olduğu için)
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                dragCursorPoint = Cursor.Position;
                dragFormPoint = this.Location;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            dragging = false;
        }
    }
}