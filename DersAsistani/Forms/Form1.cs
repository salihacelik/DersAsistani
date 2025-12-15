using System;
using System.Drawing;
using System.Windows.Forms;
using DersAsistani.Utils;

namespace DersAsistani.Forms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeForm();
            InitializeControls();
        }

        private void InitializeForm()
        {
            this.Text = "Ders Asistanı";
            this.Size = new Size(450, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(240, 242, 245);
            UIHelper.ApplyFormTheme(this);
        }

        private void InitializeControls()
        {
            // Ana panel
            Panel mainPanel = new Panel
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

            // En üstte başlık
            Label lblTitle = new Label
            {
                Text = "Ders Asistanı",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(0, 50)
            };
            
            // Ortalama işlemini Load event'inde yap
            this.Load += (s, e) =>
            {
                lblTitle.Left = (this.Width - lblTitle.Width) / 2;
            };

            // Profil resmi (boş, yuvarlak)
            PictureBox picProfile = new PictureBox
            {
                Size = new Size(120, 120),
                Location = new Point(165, 120),
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            DrawProfilePicture(picProfile);

            // Giriş butonu
            Button btnLogin = new Button
            {
                Text = "Giriş Yap",
                Location = new Point(75, 280),
                Size = new Size(300, 50),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(52, 152, 219),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            btnLogin.Click += BtnLogin_Click;

            // Kayıt butonu
            Button btnRegister = new Button
            {
                Text = "Kayıt Ol",
                Location = new Point(75, 350),
                Size = new Size(300, 50),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 1;
            btnRegister.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            btnRegister.FlatAppearance.MouseOverBackColor = Color.FromArgb(248, 249, 250);
            btnRegister.Click += BtnRegister_Click;

            // Kapatma butonu
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

            mainPanel.Controls.AddRange(new Control[]
            {
                lblTitle, picProfile, btnLogin, btnRegister
            });

            this.Controls.Add(mainPanel);
            this.Controls.Add(btnClose);
        }

        private void DrawProfilePicture(PictureBox pic)
        {
            Bitmap bmp = new Bitmap(120, 120);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                
                // Yuvarlak arka plan (açık gri)
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(236, 240, 241)))
                {
                    g.FillEllipse(brush, 0, 0, 120, 120);
                }
                
                // Yuvarlak kenarlık
                using (Pen pen = new Pen(Color.FromArgb(189, 195, 199), 2))
                {
                    g.DrawEllipse(pen, 1, 1, 118, 118);
                }
                
                // Profil ikonu (basit kullanıcı ikonu)
                using (SolidBrush iconBrush = new SolidBrush(Color.FromArgb(149, 165, 166)))
                {
                    // Kafa (yuvarlak)
                    g.FillEllipse(iconBrush, 40, 35, 40, 40);
                    
                    // Vücut (yuvarlak alt kısım)
                    g.FillEllipse(iconBrush, 25, 70, 70, 50);
                }
            }
            pic.Image = bmp;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var lf = new LoginForm();
                if (lf.ShowDialog() == DialogResult.OK)
                {
                    this.Hide();
                    var mf = new MainForm(lf.LoggedInUser);
                    mf.FormClosed += MainForm_Closed;
                    mf.Show();
                }
                lf.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Giriş formu açılırken hata oluştu: " + ex.Message, 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_Closed(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                var rf = new RegisterForm();
                if (rf.ShowDialog() == DialogResult.OK)
                {
                    // Kayıt başarılı, giriş formunu aç
                    rf.Dispose();
                    BtnLogin_Click(sender, e);
                }
                else
                {
                    rf.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kayıt formu açılırken hata oluştu: " + ex.Message, 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Form hareket ettirme
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}