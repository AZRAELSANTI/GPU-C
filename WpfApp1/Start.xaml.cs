using System;
using System.Windows;
using System.Windows.Threading;

namespace WpfApp1
{
    public partial class Start : Window
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public Start()
        {
            InitializeComponent();
            dispatcherTimer.Tick += new EventHandler(MyStart);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 13);
            dispatcherTimer.Start();
        }
        private void MyStart(object sender, EventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            dispatcherTimer.Stop();
            this.Close();
        }
    }
}