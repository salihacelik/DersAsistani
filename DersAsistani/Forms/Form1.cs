using System;
using System.Windows.Forms;

namespace DersAsistani.Forms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            // Eğer Designer kullanmıyorsan bu satırı kaldırabilirsin
            // InitializeComponent();

            this.Text = "Ders Asistanı - Karşılama";
            this.Width = 420;
            this.Height = 240;

            var btnLogin = new Button();
            btnLogin.Text = "Giriş Yap";
            btnLogin.Left = 50;
            btnLogin.Top = 60;
            btnLogin.Width = 120;
            btnLogin.Height = 40;

            var btnRegister = new Button();
            btnRegister.Text = "Kaydol";
            btnRegister.Left = 200;
            btnRegister.Top = 60;
            btnRegister.Width = 120;
            btnRegister.Height = 40;

            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += BtnRegister_Click;

            this.Controls.Add(btnLogin);
            this.Controls.Add(btnRegister);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            using (var lf = new LoginForm())
            {
                if (lf.ShowDialog() == DialogResult.OK)
                {
                    this.Hide();
                    var mf = new MainForm(lf.LoggedInUser);
                    mf.FormClosed += MainForm_Closed;
                    mf.Show();
                }
            }
        }

        private void MainForm_Closed(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            using (var rf = new RegisterForm())
            {
                rf.ShowDialog();
            }
        }
    }
}