// Utils/UIHelper.cs
using System.Drawing;
using System.Windows.Forms;

namespace DersAsistani.Utils
{
    public static class UIHelper
    {
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

        public static void StyleGrid(DataGridView grid)
        {
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.EnableHeadersVisualStyles = false;

            grid.DefaultCellStyle.BackColor = Color.FromArgb(46, 52, 58);
            grid.DefaultCellStyle.ForeColor = Color.Gainsboro;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(64, 129, 187);
        }

        public static void ApplyFormTheme(Form form)
        {
            form.BackColor = Color.FromArgb(35, 40, 45);
            form.Font = new Font("Segoe UI", 10F);
            form.ForeColor = Color.Gainsboro;
        }
    }
}