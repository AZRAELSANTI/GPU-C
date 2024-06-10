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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using System.Security.AccessControl;




namespace WpfApp1
{

    public partial class MainWindow
    {
        private Application wordApp;
        private Document doc;
        private Test test;
       
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
            test = new Test();
           
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
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, "output.txt");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/C diskpart",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = false
            };

            Process process = new Process { StartInfo = startInfo };
            process.Start();

            process.StandardInput.WriteLine("list disk");
            process.StandardInput.WriteLine("select disk 0"); // Заменить 0 на номер нужного диска
            process.StandardInput.WriteLine("detail disk");
            process.StandardInput.WriteLine("exit");

            string output = process.StandardOutput.ReadToEnd();

            // Очистка лишних символов DiskPart
            string cleanedOutput = output.Substring(output.IndexOf("DISKPART>") + 9).Trim();

            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(cleanedOutput);
            }

            process.WaitForExit();
            process.Close();

            MessageBox.Show("Результат выполнения команды diskpart сохранен на рабочем столе в файле output.txt", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
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





