using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading.Tasks;
using DersAsistani.Data;
using DersAsistani.Services;

namespace DersAsistani.Forms
{
    public class RegisterForm : Form
    {
        private Panel mainPanel;
        private Panel registerPanel;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtFullName;
        private Button btnRegister;
        private Button btnShowPassword;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblUsername;
        private Label lblPassword;
        private Label lblFullName;
        private PictureBox picLogo;
        private LinkLabel lnkLogin;
        private ProgressBar progressBar;
        private Label lblError;

        public RegisterForm()
        {
            InitializeForm();
            InitializeControls();
            SetupLayout();
            AttachEvents();
        }

        private void InitializeForm()
        {
            this.Text = "Ders Asistanı - Kayıt Ol";
            this.Size = new Size(450, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Padding = new Padding(0);
            this.MinimumSize = new Size(450, 700);
            this.MaximumSize = new Size(450, 700);
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
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    mainPanel.ClientRectangle,
                    Color.FromArgb(240, 242, 245),
                    Color.FromArgb(220, 230, 240),
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, mainPanel.ClientRectangle);
                }
            };

            // Kayıt paneli (kart görünümü)
            registerPanel = new Panel
            {
                Size = new Size(400, 650),
                BackColor = Color.White,
                Location = new Point(25, 25)
            };

            // Profil resmi (boş, yuvarlak)
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
            this.Load += (s, e) =>
            {
                lblTitle.Left = (registerPanel.Width - lblTitle.Width) / 2;
            };

            // Alt başlık
            lblSubtitle = new Label
            {
                Text = "Hesabınızı oluşturun",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(0, 170)
            };
            this.Load += (s, e) =>
            {
                lblSubtitle.Left = (registerPanel.Width - lblSubtitle.Width) / 2;
            };

            // Kullanıcı adı etiketi
            lblUsername = new Label
            {
                Text = "Kullanıcı Adı",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(50, 220)
            };

            // Kullanıcı adı textbox
            txtUsername = new TextBox
            {
                Location = new Point(50, 245),
                Size = new Size(300, 40),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250),
                ForeColor = Color.FromArgb(52, 73, 94),
                Padding = new Padding(10, 0, 10, 0)
            };
            txtUsername.Enter += (s, e) => { txtUsername.BackColor = Color.White; };
            txtUsername.Leave += (s, e) => { txtUsername.BackColor = Color.FromArgb(248, 249, 250); };

            // Ad-Soyad etiketi
            lblFullName = new Label
            {
                Text = "Ad-Soyad",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(50, 300)
            };

            // Ad-Soyad textbox
            txtFullName = new TextBox
            {
                Location = new Point(50, 325),
                Size = new Size(300, 40),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250),
                ForeColor = Color.FromArgb(52, 73, 94),
                Padding = new Padding(10, 0, 10, 0)
            };
            txtFullName.Enter += (s, e) => { txtFullName.BackColor = Color.White; };
            txtFullName.Leave += (s, e) => { txtFullName.BackColor = Color.FromArgb(248, 249, 250); };

            // Şifre etiketi
            lblPassword = new Label
            {
                Text = "Şifre",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(50, 380)
            };

            // Şifre paneli (textbox + göster butonu için)
            Panel passwordPanel = new Panel
            {
                Location = new Point(50, 405),
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
                BackColor = Color.FromArgb(248, 249, 250),
                ForeColor = Color.FromArgb(52, 73, 94),
                UseSystemPasswordChar = true,
                Dock = DockStyle.Left,
                Padding = new Padding(10, 0, 0, 0)
            };
            txtPassword.Enter += (s, e) =>
            {
                passwordPanel.BackColor = Color.White;
                txtPassword.BackColor = Color.White;
            };
            txtPassword.Leave += (s, e) =>
            {
                passwordPanel.BackColor = Color.FromArgb(248, 249, 250);
                txtPassword.BackColor = Color.FromArgb(248, 249, 250);
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
                Location = new Point(50, 460),
                Size = new Size(300, 20),
                Visible = false
            };

            // Kayıt butonu
            btnRegister = new Button
            {
                Text = "Kayıt Ol",
                Location = new Point(50, 500),
                Size = new Size(300, 45),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(52, 152, 219),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            btnRegister.FlatAppearance.MouseDownBackColor = Color.FromArgb(32, 102, 148);
            btnRegister.Click += BtnRegister_Click;

            // Progress bar (loading için)
            progressBar = new ProgressBar
            {
                Location = new Point(50, 500),
                Size = new Size(300, 45),
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Visible = false
            };

            // Giriş yap linki
            lnkLogin = new LinkLabel
            {
                Text = "Zaten hesabınız var mı? Giriş yapın",
                Location = new Point(0, 570),
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                LinkColor = Color.FromArgb(52, 152, 219),
                ActiveLinkColor = Color.FromArgb(41, 128, 185),
                VisitedLinkColor = Color.FromArgb(52, 152, 219),
                Cursor = Cursors.Hand
            };
            this.Load += (s, e) =>
            {
                lnkLogin.Left = (registerPanel.Width - lnkLogin.Width) / 2;
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
            btnClose.MouseEnter += (s, e) => { btnClose.ForeColor = Color.White; };
            btnClose.MouseLeave += (s, e) => { btnClose.ForeColor = Color.FromArgb(127, 140, 141); };
            btnClose.Click += (s, e) => this.Close();

            // Kontrolleri panele ekle
            registerPanel.Controls.AddRange(new Control[]
            {
                picLogo, lblTitle, lblSubtitle,
                lblUsername, txtUsername,
                lblFullName, txtFullName,
                lblPassword, passwordPanel,
                lblError, btnRegister, progressBar,
                lnkLogin
            });

            mainPanel.Controls.Add(registerPanel);
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
            registerPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, registerPanel.ClientRectangle,
                    Color.FromArgb(220, 220, 220), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(220, 220, 220), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(220, 220, 220), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(220, 220, 220), 1, ButtonBorderStyle.Solid);
            };
        }

        private void AttachEvents()
        {
            lnkLogin.LinkClicked += LnkLogin_LinkClicked;
            
            // Enter tuşu ile kayıt
            txtPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    BtnRegister_Click(btnRegister, EventArgs.Empty);
                }
            };
            
            txtFullName.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPassword.Focus();
                }
            };
            
            txtUsername.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtFullName.Focus();
                }
            };
        }

        private void BtnShowPassword_Click(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            btnShowPassword.Text = txtPassword.UseSystemPasswordChar ? "👁" : "👁‍🗨";
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            // Validasyon
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ShowError("Lütfen kullanıcı adınızı girin.");
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                ShowError("Lütfen ad-soyadınızı girin.");
                txtFullName.Focus();
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
            Task.Run(() =>
            {
                try
                {
                    var repo = new UserRepository();
                    var auth = new AuthService(repo);
                    var result = auth.Register(txtUsername.Text, txtPassword.Text, txtFullName.Text);

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

                            if (!ok)
                            {
                                ShowError(message);
                                return;
                            }

                            // Başarılı mesajı göster
                            MessageBox.Show(message, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }));
                    }
                    else
                    {
                        ShowLoading(false);

                        bool ok = result.Item1;
                        string message = result.Item2;

                        if (!ok)
                        {
                            ShowError(message);
                            return;
                        }

                        MessageBox.Show(message, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
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
                            ShowError("Kayıt sırasında bir hata oluştu: " + ex.Message);
                        }));
                    }
                    else
                    {
                        ShowLoading(false);
                        ShowError("Kayıt sırasında bir hata oluştu: " + ex.Message);
                    }
                }
            });
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
            btnRegister.Visible = !show;
            progressBar.Visible = show;
            btnRegister.Enabled = !show;
            txtUsername.Enabled = !show;
            txtFullName.Enabled = !show;
            txtPassword.Enabled = !show;
        }

        private void LnkLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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