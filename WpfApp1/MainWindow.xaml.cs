using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.ViewModel;
using Microsoft.VisualBasic;


namespace WpfApp1
{

    public partial class MainWindow : Window
    {
        public PCInfoViewModel PCInfo { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            PCInfo = new PCInfoViewModel();
            PCInfo.GetSystemInfo();
            PCInfo.GetDiskInfo();
            PCInfo.GetRamInfo();
            PCInfo.GetOSInfo();
            PCInfo.GetMotherboardInfo();
            GetRAMInfo();
            GetDiskSpaceInfo();
            UpdateProcessorInfo();
            UpdateGpuInfo();
            DataContext = this;


        }



        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) { this.DragMove(); }
        }
        private void Power_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }



        private void GetDiskSpaceInfo()
        {
            string driveName = "C";

            DriveInfo driveInfo = new DriveInfo(driveName);
            long totalSpace = driveInfo.TotalSize;
            long freeSpace = driveInfo.TotalFreeSpace;
            long usedSpace = totalSpace - freeSpace;

            double totalSpaceGB = totalSpace / (1024.0 * 1024 * 1024);
            double freeSpaceGB = freeSpace / (1024.0 * 1024 * 1024);
            double usedSpaceGB = usedSpace / (1024.0 * 1024 * 1024);

            progressBarTotal.Maximum = totalSpaceGB;
            progressBarTotal.Value = totalSpaceGB;
            textBlockTotal.Text = $"{totalSpaceGB:F2} GB";

            progressBarFree.Maximum = totalSpaceGB;
            progressBarFree.Value = freeSpaceGB;
            textBlockFree.Text = $"{freeSpaceGB:F2} GB";

            progressBarUsed.Maximum = totalSpaceGB;
            progressBarUsed.Value = usedSpaceGB;
            textBlockUsed.Text = $"{usedSpaceGB:F2} GB";
        }

        private void GetRAMInfo()
        {
            RAMInfo ramInfo = new RAMInfo();

            double totalMemoryGB = ramInfo.GetTotalMemoryInGB();
            double usedMemoryGB = ramInfo.GetUsedMemoryInGB();
            double freeMemoryGB = ramInfo.GetFreeMemoryInGB();

            totalProgressBar.Value = (usedMemoryGB / totalMemoryGB) * 100;
            usedProgressBar.Value = (usedMemoryGB / totalMemoryGB) * 100;
            freeProgressBar.Value = (freeMemoryGB / totalMemoryGB) * 100;

            // Update text blocks to display memory information in GB
            totalLabel.Text = $" {totalMemoryGB:F2} GB";
            usedLabel.Text = $" {usedMemoryGB:F2} GB";
            freeLabel.Text = $" {freeMemoryGB:F2} GB";
        }

        private void UpdateProcessorInfo()
        {
            ProcessorInfo processorInfo = new ProcessorInfo();

            string processorTemperature = processorInfo.GetProcessorTemperature();
            int processorPowerUsage = processorInfo.GetProcessorPowerUsage();

            lblTemperature.Content = "Processor Temperature: " + processorTemperature + "°C";
            lblPowerUsage.Content = "Processor Power Usage: " + processorPowerUsage + "%";
        }
        private void UpdateGpuInfo()
        {
            GpuInfo gpuInfo = new GpuInfo();

            string gpuTemperature = gpuInfo.GetGpuTemperature();
            int gpuUtilization = gpuInfo.GetGpuUtilization();

            lblGpuTemperature.Content = "GPU Temperature: " + gpuTemperature;
            lblGpuUtilization.Content = "GPU Utilization: " + gpuUtilization + "%";
        }




    }
}



    

