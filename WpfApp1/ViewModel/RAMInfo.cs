using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModel
{
    public class RAMInfo
    {
        public double GetTotalMemoryInGB()
        {
            ManagementObjectSearcher memorySearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
            ulong totalMemoryBytes = 0;

            foreach (ManagementObject obj in memorySearcher.Get())
            {
                totalMemoryBytes += Convert.ToUInt64(obj["Capacity"]);
            }

            double totalMemoryGB = totalMemoryBytes / (1024.0 * 1024.0 * 1024.0); // Convert bytes to gigabytes
            return totalMemoryGB;
        }

        public double GetFreeMemoryInGB()
        {
            ManagementObjectSearcher memorySearcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            ulong freeMemoryBytes = 0;

            foreach (ManagementObject obj in memorySearcher.Get())
            {
                freeMemoryBytes = Convert.ToUInt64(obj["FreePhysicalMemory"]);
            }

            double freeMemoryGB = freeMemoryBytes / (1024.0 * 1024.0 * 1024.0); // Convert bytes to gigabytes
            return freeMemoryGB;
        }

        public double GetUsedMemoryInGB()
        {
            double totalMemoryGB = GetTotalMemoryInGB();
            double freeMemoryGB = GetFreeMemoryInGB();
            double usedMemoryGB = totalMemoryGB - freeMemoryGB;
            return usedMemoryGB;
        }
    }
}
