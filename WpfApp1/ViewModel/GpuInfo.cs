using LibreHardwareMonitor.Hardware;
using System;
using System.Linq;
using System.Management;

namespace WpfApp1.ViewModel
{
    public class GpuInfo
    {
        public string GetGpuTemperature()
        {
            Computer computer = new Computer();
            computer.Open();
            computer.IsGpuEnabled = true;
            var gpu = computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.GpuNvidia || h.HardwareType == HardwareType.GpuAmd || h.HardwareType == HardwareType.GpuIntel);

            if (gpu != null && gpu.Sensors.Length > 0)
            {
                var temperatureSensor = gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);

                if (temperatureSensor != null)
                {
                    float temperatureCelsius = temperatureSensor.Value ?? 0;
                    return temperatureCelsius.ToString("0.0") + "°C";
                }
            }

            return "Temperature information not available";
        }
        public int GetGpuUtilization()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", 
                "SELECT * FROM Win32_PerfFormattedData_GPUPerformanceCounters_GPUEngine");

            foreach (ManagementObject obj in searcher.Get())
            {
                if (obj["UtilizationPercentage"] != null)
                {
                    int utilization = Convert.ToInt32(obj["UtilizationPercentage"]);
                    return utilization;
                }
            }

            return -1;
        }
    }
}
