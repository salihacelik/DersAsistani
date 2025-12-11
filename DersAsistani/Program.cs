using System;
using System.Windows.Forms;
using DersAsistani.Data;
using DersAsistani.Forms;

namespace DersAsistani
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Veritabanı dosyası ve tabloları oluştur
            Database.EnsureCreated();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}