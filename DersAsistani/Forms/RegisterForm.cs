using System;
using System.Windows.Forms;
using DersAsistani.Data;
using DersAsistani.Services;

namespace DersAsistani.Forms
{
    public class RegisterForm : Form
    {
        private TextBox txtUsername = new TextBox { Left = 120, Top = 20, Width = 180 };
        private TextBox txtPassword = new TextBox { Left = 120, Top = 60, Width = 180, UseSystemPasswordChar = true };
        private TextBox txtFullName = new TextBox { Left = 120, Top = 100, Width = 180 };
        private Button btnRegister = new Button { Text = "Kaydol", Left = 120, Top = 140, Width = 180 };

        public RegisterForm()
        {
            this.Text = "Kaydol";
            this.Width = 340;
            this.Height = 240;

            this.Controls.Add(new Label { Text = "Kullanıcı Adı:", Left = 20, Top = 22, Width = 100 });
            this.Controls.Add(new Label { Text = "Şifre:", Left = 20, Top = 62, Width = 100 });
            this.Controls.Add(new Label { Text = "Ad-Soyad:", Left = 20, Top = 102, Width = 100 });
            this.Controls.Add(txtUsername);
            this.Controls.Add(txtPassword);
            this.Controls.Add(txtFullName);
            this.Controls.Add(btnRegister);

            btnRegister.Click += BtnRegister_Click;
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            var repo = new UserRepository();
            var auth = new AuthService(repo);
            var result = auth.Register(txtUsername.Text, txtPassword.Text, txtFullName.Text);

            bool ok = result.Item1;
            string message = result.Item2;

            MessageBox.Show(message, ok ? "Bilgi" : "Hata",
                MessageBoxButtons.OK,
                ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);

            if (ok)
                this.DialogResult = DialogResult.OK;
        }
    }
}