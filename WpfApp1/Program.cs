using System.Management;
using System.Text;

namespace WpfApp1
{
    class Program
    {
        public static string GetSystemInfo()
        {
            StringBuilder systemInfo = new StringBuilder();

            ManagementObjectSearcher gpuSearcher = new ManagementObjectSearcher("select * from Win32_VideoController");
            ManagementObjectCollection gpuCollection = gpuSearcher.Get();
            foreach (ManagementObject gpu in gpuCollection)
            {
                systemInfo.AppendLine($"GPU: {gpu["Name"]}");

                long videoMemoryBytes = long.Parse(gpu["AdapterRAM"].ToString());
                double videoMemoryGB = videoMemoryBytes / 1024.0 / 1024.0 / 1024.0;
                systemInfo.AppendLine($"Video memory: {videoMemoryGB:F2} GB");
            }


            ManagementObjectSearcher cpuSearcher = new ManagementObjectSearcher("select * from Win32_Processor");
            ManagementObjectCollection cpuCollection = cpuSearcher.Get();
            foreach (ManagementObject cpu in cpuCollection)
            {
                systemInfo.AppendLine($"CPU: {cpu["Name"]}");
                systemInfo.AppendLine($"Number of cores: {cpu["NumberOfCores"]}");
                systemInfo.AppendLine($"Frequency: {cpu["CurrentClockSpeed"]} MHz");
                systemInfo.AppendLine($"Load: {cpu["LoadPercentage"]}%");
            }


            ManagementObjectSearcher ramSearcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
            ManagementObjectCollection ramCollection = ramSearcher.Get();
            long totalRam = 0; // Суммируем общий объем оперативы
            foreach (ManagementObject ram in ramCollection)
            {
                totalRam += long.Parse(ram["Capacity"].ToString());
            }
            systemInfo.AppendLine($"RAM: {totalRam / 1024 / 1024 / 1024:F2} GB"); // Вывод в ГБ


            ManagementObjectSearcher hardDiskSearcher = new ManagementObjectSearcher("select * from Win32_DiskDrive");
            ManagementObjectCollection hardDiskCollection = hardDiskSearcher.Get();
            foreach (ManagementObject hardDisk in hardDiskCollection)
            {
                systemInfo.AppendLine($"Disk Model: {hardDisk["Model"]}");

                // Преобразование байт в ГБ
                //long freeSpaceBytes = long.Parse(hardDisk["AvailableFreeSpace"].ToString());
                //double freeSpaceGB = freeSpaceBytes / 1024.0 / 1024.0 / 1024.0;

                long totalSpaceBytes = long.Parse(hardDisk["Size"].ToString());
                double totalSpaceGB = totalSpaceBytes / 1024.0 / 1024.0 / 1024.0;

                //systemInfo.AppendLine($"Free space: {freeSpaceGB:F2} GB");
                systemInfo.AppendLine($"Total space: {totalSpaceGB:F2} GB");
            }

            ManagementObjectSearcher osSearcher = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            ManagementObjectCollection osCollection = osSearcher.Get();
            foreach (ManagementObject os in osCollection)
            {
                systemInfo.AppendLine($"Operating system: {os["Name"]}");
            }

            return systemInfo.ToString();




        }
    }
}


