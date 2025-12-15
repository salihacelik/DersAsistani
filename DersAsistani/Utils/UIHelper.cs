// Utils/UIHelper.cs
using System.Drawing;
using System.Windows.Forms;

namespace DersAsistani.Utils
{
    public static class UIHelper
    {
        public enum ThemeMode
        {
            Dark,
            Light
        }

        public static void StyleButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 10F);
            btn.ForeColor = Color.White;
            btn.BackColor = Color.Transparent;

            btn.MouseEnter += delegate { btn.BackColor = Color.FromArgb(220, 240, 255); btn.ForeColor = Color.Black; };
            btn.MouseLeave += delegate { btn.BackColor = Color.Transparent; btn.ForeColor = Color.White; };
        }

        public static void StyleGrid(DataGridView grid, ThemeMode theme = ThemeMode.Dark)
        {
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.EnableHeadersVisualStyles = false;
            grid.BorderStyle = BorderStyle.None;

            if (theme == ThemeMode.Dark)
            {
                grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
                grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

                grid.DefaultCellStyle.BackColor = Color.FromArgb(46, 52, 58);
                grid.DefaultCellStyle.ForeColor = Color.Gainsboro;
                grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(64, 129, 187);
                grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(40, 45, 50);
            }
            else
            {
                grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 249);
                grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(52, 73, 94);

                grid.DefaultCellStyle.BackColor = Color.White;
                grid.DefaultCellStyle.ForeColor = Color.FromArgb(52, 73, 94);
                grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 230, 250);
                grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            }
        }

        public static void ApplyFormTheme(Form form, ThemeMode theme = ThemeMode.Dark)
        {
            form.Font = new Font("Segoe UI", 10F);

            if (theme == ThemeMode.Dark)
            {
                form.BackColor = Color.FromArgb(35, 40, 45);
                form.ForeColor = Color.Gainsboro;
            }
            else
            {
                // Light theme: yalnızca form arka planı ve foreground ayarlanır,
                // diğer paneller/başlıkların stillerini MainForm kendisi belirlesin.
                form.BackColor = Color.FromArgb(240, 242, 245);
                form.ForeColor = Color.FromArgb(52, 73, 94);
            }
        }
    }
}