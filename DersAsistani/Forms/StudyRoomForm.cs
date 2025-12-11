using System;
using System.IO;
using System.Windows.Forms;

namespace DersAsistani.Forms
{
    public class StudyRoomForm : Form
    {
        private Label lblTimer = new Label { Left = 20, Top = 20, Width = 200, Text = "Geçen süre: 00:00:00" };
        private Button btnStart = new Button { Left = 20, Top = 60, Width = 80, Text = "Başla" };
        private Button btnStop = new Button { Left = 120, Top = 60, Width = 80, Text = "Bitir" };

        private Timer timer;
        private DateTime startTime;

        public StudyRoomForm()
        {
            this.Text = "Çalışma Odası";
            this.Width = 300;
            this.Height = 150;

            this.Controls.Add(lblTimer);
            this.Controls.Add(btnStart);
            this.Controls.Add(btnStop);

            timer = new Timer();
            timer.Interval = 1000; // her saniye
            timer.Tick += Timer_Tick;

            btnStart.Click += BtnStart_Click;
            btnStop.Click += BtnStop_Click;
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            startTime = DateTime.Now;
            timer.Start();
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            timer.Stop();
            var end = DateTime.Now;
            TimeSpan elapsed = end - startTime;

            // Günlük dosya yolu
            string day = DateTime.Now.ToString("yyyy-MM-dd");
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            string file = Path.Combine(dir, "study_sessions_" + day + ".csv");

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            bool writeHeader = !File.Exists(file);

            using (var sw = new StreamWriter(file, true))
            {
                if (writeHeader) sw.WriteLine("Date,StartTime,EndTime,Duration");
                sw.WriteLine(string.Format("{0},{1},{2},{3}",
                    day,
                    startTime.ToString("HH:mm:ss"),
                    end.ToString("HH:mm:ss"),
                    elapsed.ToString(@"hh\:mm\:ss")));
            }

            MessageBox.Show("Toplam çalışma süresi: " + elapsed.ToString(@"hh\:mm\:ss"),
                "Çalışma Bitti", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - startTime;
            lblTimer.Text = "Geçen süre: " + elapsed.ToString(@"hh\:mm\:ss");
        }
    }
}