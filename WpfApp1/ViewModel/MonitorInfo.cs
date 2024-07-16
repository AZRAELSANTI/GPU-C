using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfApp1.ViewModel
{
    public class MonitorInfo
    {
        public string Name { get; set; }
        public string Resolution { get; set; }
        public string Model { get; set; }
    

        public MonitorInfo GetMonitorInfo()
        {
            var monitorInfo = new MonitorInfo();

           
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DesktopMonitor");


            var monitorObject = searcher.Get().Cast<ManagementObject>().FirstOrDefault();
            Screen primaryScreen = Screen.PrimaryScreen;
            if (monitorObject != null)
            {


                // Get the screen resolution
                int screenWidth = primaryScreen.Bounds.Width;
                int screenHeight = primaryScreen.Bounds.Height;


                monitorInfo.Name = monitorObject["Name"].ToString();

                monitorInfo.Resolution = $"{screenWidth} x {screenHeight}";

               
                monitorInfo.Model = $"{monitorObject["MonitorManufacturer"]}";

               
                
            }

            return monitorInfo;
        }
    }
}

