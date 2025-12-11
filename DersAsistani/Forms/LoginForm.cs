using System;
using System.Windows.Forms;
using DersAsistani.Data;
using DersAsistani.Models;
using DersAsistani.Services;

namespace DersAsistani.Forms
{
    public class LoginForm : Form
    {
        private TextBox txtUsername = new TextBox { Left = 120, Top = 20, Width = 180 };
        private TextBox txtPassword = new TextBox { Left = 120, Top = 60, Width = 180, UseSystemPasswordChar = true };
        private Button btnLogin = new Button { Text = "Giriş Yap", Left = 120, Top = 100, Width = 180 };

        public User LoggedInUser { get; private set; }

        public LoginForm()
        {
            this.Text = "Giriş Yap";
            this.Width = 340;
            this.Height = 200;

            this.Controls.Add(new Label { Text = "Kullanıcı Adı:", Left = 20, Top = 22, Width = 100 });
            this.Controls.Add(new Label { Text = "Şifre:", Left = 20, Top = 62, Width = 100 });
            this.Controls.Add(txtUsername);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);

            btnLogin.Click += BtnLogin_Click;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            var repo = new UserRepository();
            var auth = new AuthService(repo);
            var result = auth.Login(txtUsername.Text, txtPassword.Text);

            bool ok = result.Item1;
            string message = result.Item2;
            User user = result.Item3;

            if (!ok)
            {
                MessageBox.Show(message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoggedInUser = user;
            this.DialogResult = DialogResult.OK;
        }
    }
}