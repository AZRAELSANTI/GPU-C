using System;
using System.Management;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using HtmlAgilityPack;

namespace WpfApp1
{


    public class Drivers
    {
        public void GetDriverInfo()
        {
                string currentDriverVersion = GetInstalledDriverVersion();

                string url = "https://www.amd.com/ru/support";
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);

                HtmlNode driverNode = doc.DocumentNode.SelectSingleNode("//div[@class='latest-driver']");
                string latestDriverVersion = driverNode.InnerText.Trim();

                string message = $"Текущий драйвер: {currentDriverVersion}\nНовейший драйвер на сайте AMD: {latestDriverVersion}";

                MessageBox.Show(message, "Информация о драйверах");
            }
            private string GetInstalledDriverVersion()
        {
            string query = "SELECT * FROM Win32_PnPSignedDriver WHERE DeviceClass = 'Display'";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection results = searcher.Get();

            foreach (ManagementObject result in results)
            {
                string driverDescription = result["Description"].ToString();
                if (driverDescription.Contains("AMD"))
                {
                    return result["DriverVersion"].ToString();
                }
            }

            return "Не удалось определить версию драйвера";
        }

    }
}





