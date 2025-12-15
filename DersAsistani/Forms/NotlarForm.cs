using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DersAsistani.Models;
using DersAsistani.Services;

namespace DersAsistani.Forms
{
    public class NotlarForm : Form
    {
        // UI Bileşenleri
        private Panel pnlInput;     // Üst Kısım
        private FlowLayoutPanel flowPanel; // Alt Kısım (Notlar)

        private TextBox txtTitle;
        private TextBox txtContent;
        private Button btnAdd;

        private NoteService _service;

        public NotlarForm()
        {
            // 1. Form Ayarları
            this.Text = "Not Defterim";
            this.Size = new Size(950, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            // Form boyutu değişince düzen bozulmasın
            this.Resize += NotlarForm_Resize;

            _service = new NoteService();

            InitializeCustomControls();
            LoadNotes();
        }

        private void InitializeCustomControls()
        {
            // --- 1. ÜST PANEL (Giriş Alanı) ---
            pnlInput = new Panel
            {
                Dock = DockStyle.Top, // En tepeye sabitle
                Height = 120,         // Yüksekliği kesin belirle
                BackColor = Color.White,
                Padding = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Başlık
            Label lblTitle = new Label { Text = "Konu:", Left = 20, Top = 20, AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            txtTitle = new TextBox { Left = 80, Top = 17, Width = 250, Font = new Font("Segoe UI", 11) };
            SetPlaceholder(txtTitle, "Başlık...");

            // İçerik
            Label lblContent = new Label { Text = "Not:", Left = 350, Top = 20, AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            txtContent = new TextBox { Left = 400, Top = 17, Width = 380, Height = 80, Multiline = true, Font = new Font("Segoe UI", 10), ScrollBars = ScrollBars.Vertical };
            SetPlaceholder(txtContent, "Notunuzu buraya yazın...");

            // Kaydet Butonu
            btnAdd = new Button
            {
                Text = "Notu Kaydet",
                Left = 800,
                Top = 17,
                Width = 110,
                Height = 80,
                BackColor = Color.LightSeaGreen,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAdd.Click += BtnAdd_Click;

            // Panele ekle
            pnlInput.Controls.Add(lblTitle);
            pnlInput.Controls.Add(txtTitle);
            pnlInput.Controls.Add(lblContent);
            pnlInput.Controls.Add(txtContent);
            pnlInput.Controls.Add(btnAdd);

            // --- 2. ALT PANEL (Not Kutuları) ---
            // Dock=Fill kullanmıyoruz, elle konumlandırıyoruz ki kayma olmasın.
            flowPanel = new FlowLayoutPanel
            {
                Location = new Point(0, 120), // Tam olarak üst panelin bittiği yerden başla
                Width = this.ClientSize.Width,
                Height = this.ClientSize.Height - 120, // Kalan yüksekliği al
                AutoScroll = true,
                Padding = new Padding(20), // Kenarlardan boşluk bırak
                BackColor = Color.WhiteSmoke,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right // Pencere büyürse bu da büyüsün
            };

            // Forma Ekle
            this.Controls.Add(pnlInput);
            this.Controls.Add(flowPanel);
        }

        // Form boyutu değişince listeyi yeniden ayarla
        private void NotlarForm_Resize(object sender, EventArgs e)
        {
            if (flowPanel != null)
            {
                flowPanel.Width = this.ClientSize.Width;
                flowPanel.Height = this.ClientSize.Height - 120;
            }
        }

        private void LoadNotes()
        {
            flowPanel.Controls.Clear();
            List<Note> notes = _service.GetAll();

            // Not yoksa uyarı göster
            if (notes.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "Henüz hiç notunuz yok. Yukarıdan ekleyebilirsiniz.",
                    AutoSize = true,
                    Font = new Font("Segoe UI", 12, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    Padding = new Padding(20)
                };
                flowPanel.Controls.Add(lblEmpty);
            }
            else
            {
                foreach (var note in notes)
                {
                    flowPanel.Controls.Add(CreateNoteCard(note));
                }
            }
        }

        // --- SARI NOT KAĞIDI TASARIMI ---
        private Panel CreateNoteCard(Note note)
        {
            Panel card = new Panel
            {
                Width = 240,
                Height = 240,
                BackColor = Color.FromArgb(255, 250, 205), // Limon Sarısı (LemonChiffon)
                Margin = new Padding(15),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Başlık Çubuğu (Koyu Şerit)
            Label lblHeader = new Label
            {
                Dock = DockStyle.Top,
                Height = 35,
                BackColor = Color.FromArgb(240, 230, 140), // Koyu Sarı
                Text = "  " + note.Title,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkSlateGray
            };

            // Silme Butonu (X)
            Label btnClose = new Label
            {
                Text = "✕",
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                AutoSize = true,
                Parent = lblHeader, // Başlık şeridinin üstünde dursun
                Left = 210,
                Top = 8
            };
            // X butonunu en öne getir
            btnClose.BringToFront();

            btnClose.Click += (s, e) =>
            {
                if (MessageBox.Show("Notu silmek istiyor musunuz?", "Sil", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _service.Delete(note.Id);
                    LoadNotes();
                }
            };

            // X butonunu header'a ekle
            lblHeader.Controls.Add(btnClose);

            // İçerik Yazısı
            Label lblBody = new Label
            {
                Text = note.Content,
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.TopLeft
            };

            // Sırayla ekle
            card.Controls.Add(lblBody);
            card.Controls.Add(lblHeader); // Header'ı en son ekle ki üstte dursun

            return card;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || txtTitle.Text == "Başlık...")
            {
                MessageBox.Show("Lütfen bir başlık giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string content = txtContent.Text == "Notunuzu buraya yazın..." ? "" : txtContent.Text;

            var result = _service.Create(txtTitle.Text, content);
            if (result.Item1)
            {
                LoadNotes();
                ResetInput(txtTitle, "Başlık...");
                ResetInput(txtContent, "Notunuzu buraya yazın...");
            }
            else
            {
                MessageBox.Show("Hata: " + result.Item2);
            }
        }

        private void SetPlaceholder(TextBox txt, string placeholder)
        {
            txt.Text = placeholder;
            txt.ForeColor = Color.Gray;
            txt.GotFocus += (s, e) => { if (txt.Text == placeholder) { txt.Text = ""; txt.ForeColor = Color.Black; } };
            txt.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txt.Text)) { txt.Text = placeholder; txt.ForeColor = Color.Gray; } };
        }
        private void ResetInput(TextBox txt, string ph) { txt.Text = ph; txt.ForeColor = Color.Gray; }
    }
}