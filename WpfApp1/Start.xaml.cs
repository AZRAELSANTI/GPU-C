using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Start.xaml
    /// </summary>
    public partial class Start : Window
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public Start()
        {
            InitializeComponent();
            dispatcherTimer.Tick += new EventHandler(MyStart);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 6);
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