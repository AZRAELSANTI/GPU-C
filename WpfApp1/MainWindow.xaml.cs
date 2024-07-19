using Microsoft.Office.Interop.Word;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WpfApp1.ViewModel;
using Application = Microsoft.Office.Interop.Word.Application;
using System.Diagnostics;
using Microsoft.Win32;
using System.Management;
using System.Text.RegularExpressions;





namespace WpfApp1
{

    public partial class MainWindow
    {
        private Test test;
        public PCInfoViewModel PCInfo { get; set; }
        public string MonitorInfo { get; set; }
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
            test = new Test();
            NetworkInfo();
            GetGPUInfo();
            Monitor();
            lblGPUClockSpeed.Content = "VRAM Clock: " + ClockSpeed + "MHz";
            lblGPUMemoryClockSpeed.Content = "GPU Clock: " +MemoryClockSpeed + "MHz";
        }

        public long MemoryUsed { get; set; }
        public int ClockSpeed { get; set; }
        public int MemoryClockSpeed { get; set; }



        private void GetGPUInfo()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            ManagementObjectCollection collection = searcher.Get();

            foreach (ManagementObject obj in collection)
            {
                
                ClockSpeed = int.Parse(obj["CurrentBitsPerPixel"].ToString());
                MemoryClockSpeed = int.Parse(obj["CurrentRefreshRate"].ToString());
                break;
            }
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
            string systemInfo = Program.GetSystemInfo();
            Application wordApp = new Application();
            Document wordDoc = wordApp.Documents.Add();
            wordDoc.Content.Text = systemInfo;
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "System Information.docx");
            wordDoc.SaveAs2(filePath);
            wordApp.Quit();
            MessageBox.Show("Информация о ПК была успешно записана в документ Word", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            test.RunAllStressTests();

            MessageBox.Show("Стресс тест успешно завершен!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void Registr_Click(object sender, RoutedEventArgs e)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "registry_info.docx");

            using (RegistryKey key = Registry.LocalMachine)
            {
                Application wordApp = new Application();
                {
                    Document document = wordApp.Documents.Add();

                    TraverseRegistry(key, document.Content);

                    document.SaveAs2(filePath);
                    document.Close();
                    wordApp.Quit();

                    MessageBox.Show("Информация из реестра сохранена на рабочем столе в файле 'registry_info.docx'");
                }
            }
        }
        private void DownloadDrivers_Click(object sender, RoutedEventArgs e)
        {
            Drivers drivers = new Drivers();
            drivers.CheckForDriverUpdates();

        }
        private void Bios_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("shutdown", "/r /fw /t 0");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void TraverseRegistry(RegistryKey key, Range parentRange)
        {

            foreach (string subKeyName in key.GetSubKeyNames())
            {
                using (RegistryKey subKey = key.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\"))
                {
                    if (subKey == null)
                    {
                        continue; // Пропускаем ключ, если у нас нет доступа к нему
                    }

                    Paragraph keyParagraph = parentRange.Paragraphs.Add();
                    keyParagraph.Range.Text = $"Ключ: {subKeyName}";
                    keyParagraph.Range.InsertParagraphAfter();

                    foreach (string valueName in subKey.GetValueNames())
                    {
                        object value = subKey.GetValue(valueName);
                        Paragraph valueParagraph = parentRange.Paragraphs.Add();
                        valueParagraph.Range.Text = $"\t{valueName}: {value}";
                        valueParagraph.Range.InsertParagraphAfter();
                    }

                    TraverseRegistry(subKey, parentRange); // Рекурсивный обход подключей
                }
            }
        }
        private void Apps_Click(object sender, RoutedEventArgs e)
        {

            {
                Application wordApp = new Application();
                Document document = wordApp.Documents.Add();

                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");

                Paragraph title = document.Content.Paragraphs.Add();
                title.Range.Text = "Установленные приложения";
                title.Range.Font.Bold = 1;
                title.Range.Font.Size = 16;
                title.Range.InsertParagraphAfter();

                if (key != null)
                {
                    foreach (string subKeyName in key.GetSubKeyNames())
                    {
                        RegistryKey subKey = key.OpenSubKey(subKeyName);
                        string appName = subKey.GetValue("DisplayName") as string;
                        string appVersion = subKey.GetValue("DisplayVersion") as string;

                        if (!string.IsNullOrEmpty(appName) && !string.IsNullOrEmpty(appVersion))
                        {
                            Paragraph appInfo = document.Content.Paragraphs.Add();
                            appInfo.Range.Text = $"{appName} - {appVersion}";
                            appInfo.Range.InsertParagraphAfter();
                        }
                    }
                }

                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "installed_apps.docx");
                document.SaveAs2(filePath);
                document.Close();
                wordApp.Quit();

                MessageBox.Show("Информация об установленных приложениях сохранена на рабочем столе в файле 'installed_apps.docx'");
            }
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


            totalLabel.Text = $" {totalMemoryGB:F2} GB";
            usedLabel.Text = $" {usedMemoryGB:F2} GB";
            freeLabel.Text = $" {freeMemoryGB:F2} GB";
        }

        private void UpdateProcessorInfo()
        {
            ProcessorInfo processorInfo = new ProcessorInfo();

            string processorTemperature = processorInfo.GetProcessorTemperature();
            int processorPowerUsage = processorInfo.GetProcessorPowerUsage();
            string CpuInfoRetriever = processorInfo.GetCpuSocketInformation();

            lblTemperature.Content = "CPU Temperature: " + processorTemperature + "°C";
            lblPowerUsage.Content = "CPU Power Usage: " + processorPowerUsage + "%";
            lblSocketInfo.Content = CpuInfoRetriever;
        }
        private void UpdateGpuInfo()
        {
            GpuInfo gpuInfo = new GpuInfo();

            string gpuTemperature = gpuInfo.GetGpuTemperature();
            int gpuUtilization = gpuInfo.GetGpuUtilization();

            lblGpuTemperature.Content = "GPU Temperature: " + gpuTemperature;
            lblGpuUtilization.Content = "GPU Utilization: " + gpuUtilization + "%";
        }
        private void NetworkInfo()
        {
            NetworkInfoRetriever networkInfoRetriever = new NetworkInfoRetriever();

            string pingResult = networkInfoRetriever.GetPing("www.google.com");
            lblPingResult.Content = pingResult;

            int downloadSpeedResult = networkInfoRetriever.GetMaxDownloadSpeed();
            lblDownloadSpeedResult.Content = downloadSpeedResult + " Mbps";
            long uploadSpeedResult = networkInfoRetriever.GetMaxUploadSpeed(); // 1MB file size
            lblUploadSpeedResult.Content = uploadSpeedResult + " Mbps";

        }
        private void Monitor()
        {
            var monitorInfo = new MonitorInfo().GetMonitorInfo();

            
            txtMonitorName.Text = monitorInfo.Name;
            txtMonitorRes.Text = monitorInfo.Resolution;
            txtMonitorModel.Text = monitorInfo.Model;
            
        }
       
    }
}