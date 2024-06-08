using System;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using HtmlAgilityPack;
using LibreHardwareMonitor.Hardware.Motherboard;

namespace WpfApp1
{


    public class Drivers
    {
        public void CheckForDriverUpdates()
        {
            string currentDriverVersion = GetCurrentDriverVersion();

            string manufacturer = GetGraphicsCardManufacturer();
            if (manufacturer == "AMD" || manufacturer == "NVIDIA")
            {
                string latestDriverVersion = GetLatestDriverVersion(manufacturer);
                if (latestDriverVersion == currentDriverVersion)
                {
                    MessageBox.Show("У вас последняя версия драйверов", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Есть новая версия драйвера для вашей видеокарты: {latestDriverVersion}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else if (manufacturer == "Unknown")
            {
                MessageBox.Show("Не удалось определить производителя видеокарты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Драйверы для данного производителя не поддерживаются.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private string GetCurrentDriverVersion()
        {
            string query = "SELECT * FROM Win32_PnPSignedDriver WHERE DeviceName LIKE '%Display%'";

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection results = searcher.Get();

            foreach (ManagementObject obj in results)
            {
                string driverVersion = obj["DriverVersion"] as string;
                if (!string.IsNullOrEmpty(driverVersion))
                {
                    return driverVersion;
                }
            }

            return "Не удалось получить информацию о версии драйвера";
        }

        private string GetGraphicsCardManufacturer()
        {
            if (IsAMDGraphicsCard())
            {
                return "AMD";
            }
            else if (IsNVIDIAGraphicsCard())
            {
                return "NVIDIA";
            }

            return "Unknown";
        }

        private bool IsAMDGraphicsCard()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");

            foreach (ManagementObject obj in searcher.Get())
            {
                string manufacturer = obj["AdapterCompatibility"].ToString();
                if (manufacturer.Contains("AMD"))
                {
                    return true;
                }
            }

            return false;
        }


        private bool IsNVIDIAGraphicsCard()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");

            foreach (ManagementObject obj in searcher.Get())
            {
                string manufacturer = obj["AdapterCompatibility"].ToString();
                if (manufacturer.Contains("NVIDIA"))
                {
                    return true;
                }
            }

            return false;

        }

        private static string GetLatestDriverVersion(string manufacturer)
        {
            string url = "";
            if (manufacturer == "AMD")
            {
                url = $"https://www.amd.com/en/support/graphics/";
            }
            else if (manufacturer == "NVIDIA")
            {
                url = $"https://www.nvidia.com/Download/driverResults.aspx/226798/en-us/";
            }

            WebClient client = new WebClient();
            string htmlCode = client.DownloadString(url);

            string pattern = @"(?<=Latest Driver Version:\s)(\d+\.\d+\.\d+\.\d+)";
            Match match = Regex.Match(htmlCode, pattern);

            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "Не удалось получить информацию о последней версии драйвера.";
            }
        }
    }
}

        
    





