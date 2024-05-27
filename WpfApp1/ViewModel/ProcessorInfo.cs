using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModel
{
    public class ProcessorInfo
    {
        public string GetProcessorTemperature()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PerfFormattedData_Counters_ThermalZoneInformation");

            foreach (ManagementObject obj in searcher.Get())
            {
                if (obj["Temperature"] != null)
                {
                    double temperatureKelvin = Convert.ToDouble(obj["Temperature"]);
                    double temperatureCelsius = temperatureKelvin - 273.15;
                    int roundedTemperature = (int)Math.Round(temperatureCelsius);
                    return roundedTemperature.ToString();
                }
            }

            return "Temperature information not available";
        }

        public int GetProcessorPowerUsage()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PerfFormattedData_Counters_ProcessorInformation");

            foreach (ManagementObject obj in searcher.Get())
            {
                if (obj["PercentProcessorPerformance"] != null)
                {
                    int powerUsage = Convert.ToInt32(obj["PercentProcessorPerformance"]);
                    return powerUsage;
                }
            }

            return -1; // Power usage information not available 
        }
    }
}

