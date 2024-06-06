

using OpenHardwareMonitor.Hardware;
using Microsoft.Office.Interop.Word;
using System;
using System.IO;
using System.Management;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WpfApp1.ViewModel;
using Application = Microsoft.Office.Interop.Word.Application;
using System.Diagnostics;


namespace WpfApp1
{

    public partial class MainWindow
    {
        private Application wordApp;
        private Document doc;
        private Test stressTest;
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
            stressTest = new Test();

        }



        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) { this.DragMove(); }
        }
        private void Power_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Info_Click(object sender, RoutedEventArgs e)
        {
            // Get system information
            string systemInfo = Program.GetSystemInfo();

            // Create a new Word document
            Application wordApp = new Application();
            Document wordDoc = wordApp.Documents.Add();

            // Add system information to the Word document
            wordDoc.Content.Text = systemInfo;

            // Save the Word document
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "System Information.docx");
            wordDoc.SaveAs2(filePath);

            // Close the Word application
            wordApp.Quit();

            // Show a message to the user
            MessageBox.Show("System information successfully saved to Word file.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            stressTest.RunAllStressTests();
            MessageBox.Show("Stress tests completed successfully!");
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



    

