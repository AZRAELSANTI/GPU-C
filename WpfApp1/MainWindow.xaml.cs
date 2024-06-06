using Microsoft.Office.Interop.Word;
using System;
using System.IO;
using System.Management;
using System.Windows;
using System.Windows.Input;
using WpfApp1.ViewModel;


namespace WpfApp1
{

    public partial class MainWindow
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
        private void Test_Click(object sender, RoutedEventArgs e)
        {
            // Создать новый документ Word
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            Document wordDoc = wordApp.Documents.Add();

            // Получить информацию о системе
            ManagementObjectSearcher cpuSearcher = new ManagementObjectSearcher("select * from Win32_Processor");
            ManagementObjectCollection cpuCollection = cpuSearcher.Get();
            ManagementObject cpu = cpuCollection[0];
            string cpuName = cpu["Name"].ToString();
            int cpuCores = int.Parse(cpu["NumberOfCores"].ToString());
            int cpuThreads = int.Parse(cpu["NumberOfLogicalProcessors"].ToString());

            // Получить информацию о видеокарте
            ManagementObjectSearcher gpuSearcher = new ManagementObjectSearcher("select * from Win32_VideoController");
            ManagementObjectCollection gpuCollection = gpuSearcher.Get();
            ManagementObject gpu = gpuCollection[0];
            string gpuName = gpu["Name"].ToString();
            int gpuMemory = int.Parse(gpu["AdapterRAM"].ToString());

            // Получить информацию об оперативной памяти
            ManagementObjectSearcher ramSearcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
            ManagementObjectCollection ramCollection = ramSearcher.Get();
            int ramSize = 0;
            foreach (ManagementObject ram in ramCollection)
            {
                ramSize += int.Parse(ram["Capacity"].ToString());
            }

            // Получить информацию о хранилище
            ManagementObjectSearcher romSearcher = new ManagementObjectSearcher("select * from Win32_DiskDrive");
            ManagementObjectCollection romCollection = romSearcher.Get();
            ManagementObject rom = romCollection[0];
            string romName = rom["Model"].ToString();
            long romSize = long.Parse(rom["Size"].ToString());

            // Получить информацию об операционной системе
            ManagementObjectSearcher osSearcher = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            ManagementObjectCollection osCollection = osSearcher.Get();
            ManagementObject os = osCollection[0];
            string osName = os["Name"].ToString();
            string osVersion = os["Version"].ToString();

            // Получить информацию о материнской плате
            ManagementObjectSearcher motherboardSearcher = new ManagementObjectSearcher("select * from Win32_BaseBoard");
            ManagementObjectCollection motherboardCollection = motherboardSearcher.Get();
            ManagementObject motherboard = motherboardCollection[0];
            string motherboardName = motherboard["Product"].ToString();

            // Добавить информацию о системе в документ Word
            wordDoc.Content.Text = "Информация о системе:\n\n";
            wordDoc.Content.Text += $"Процессор: {cpuName}, {cpuCores} ядер, {cpuThreads} потоков\n";
            wordDoc.Content.Text += $"Видеокарта: {gpuName}, {gpuMemory} МБ памяти\n";
            wordDoc.Content.Text += $"Оперативная память: {ramSize} МБ\n";
            wordDoc.Content.Text += $"Хранилище: {romName}, {romSize / 1024 / 1024 / 1024} ГБ\n";
            wordDoc.Content.Text += $"Операционная система: {osName}, {osVersion}\n";
            wordDoc.Content.Text += $"Материнская плата: {motherboardName}\n";

            // Сохранить документ
            string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Информация о системе.docx");
            wordDoc.SaveAs2(filePath);

            // Закрыть приложение Word
            wordApp.Quit();

            // Вывести сообщение об успехе
            MessageBox.Show("Информация о системе успешно сохранена в файл Word.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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



    

