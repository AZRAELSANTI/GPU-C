using OpenHardwareMonitor.Hardware;
using System;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
namespace WpfApp1
{
    public class Test
    {
        private Word.Application wordApp;
        private Word.Document doc;

        private Computer computer;
        private double cpuTemperature;
        private double gpuLoad;
        private int gpuClockSpeed;
        private double ramLoad;
        private double cpuLoad;

        public Test()
        {
            wordApp = new Word.Application();
            doc = wordApp.Documents.Add();
            computer = new Computer();
            computer.CPUEnabled = true;
            computer.GPUEnabled = true;
            computer.RAMEnabled = true;
            computer.Open();
        }

        public void RunAllStressTests()
        {
            RunCPUSressTest();
            RunGPUStressTest();
            RunRAMStressTest();
            SaveResultsToExcel(cpuTemperature, cpuLoad, gpuLoad, gpuClockSpeed, ramLoad);

        }

        public void RunCPUSressTest()
        {
            Console.WriteLine("Running CPU Stress Test...");
            foreach (var hardware in computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.CPU)
                {
                    hardware.Update();
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load && sensor.Name == "CPU Total")
                        {
                            cpuLoad = sensor.Value ?? 0;
                        }
                        else if (sensor.SensorType == SensorType.Temperature && sensor.Name == "CPU Package")
                        {
                            cpuTemperature = sensor.Value ?? 0;
                        }
                    }
                }
            }
            Console.WriteLine($"CPU Load: {cpuLoad}%");
            Console.WriteLine($"CPU Temperature: {cpuTemperature}°C");
            MeasureTemperature();
            Console.WriteLine("CPU Stress Test Completed.");
        }

        public void RunGPUStressTest()
        {
            Console.WriteLine("Running GPU Stress Test...");
            foreach (var hardware in computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.GpuNvidia || hardware.HardwareType == HardwareType.GpuAti)
                {
                    hardware.Update();
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Core")
                        {
                            gpuLoad = sensor.Value ?? 0;
                        }
                        else if (sensor.SensorType == SensorType.Clock && sensor.Name == "GPU Core")
                        {
                            gpuClockSpeed = (int)(sensor.Value ?? 0);
                        }
                    }
                }
            }
            Console.WriteLine($"GPU Load: {gpuLoad}%");
            Console.WriteLine($"GPU Clock Speed: {gpuClockSpeed} MHz");
            MeasureTemperature();
            Console.WriteLine("GPU Stress Test Completed.");
        }

        public void RunRAMStressTest()
        {
            Console.WriteLine("Running RAM Stress Test...");
            foreach (var hardware in computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.RAM)
                {
                    hardware.Update();
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load && sensor.Name == "Memory")
                        {
                            ramLoad = sensor.Value ?? 0;
                        }
                    }
                }
            }
            Console.WriteLine($"RAM Load: {ramLoad}%");
            MeasureTemperature();
            Console.WriteLine("RAM Stress Test Completed.");
        }

        private void MeasureTemperature()
        {

        }




        private void SaveResultsToExcel(double cpuTemperature, double cpuLoad, double gpuLoad, double gpuClockSpeed, double ramLoad)
        {
            Excel.Application excelApp = new Excel.Application();
            excelApp.Visible = true; // Сделать Excel видимым

            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = workbook.Sheets[1];

            // Добавление данных в ячейки
            worksheet.Cells[1, 1] = "Параметр";
            worksheet.Cells[1, 2] = "Значение";
            worksheet.Cells[2, 1] = "Температура процессора";
            worksheet.Cells[2, 2] = cpuTemperature;
            worksheet.Cells[3, 1] = "Загрузка процессора";
            worksheet.Cells[3, 2] = cpuLoad;
            worksheet.Cells[4, 1] = "Загрузка GPU";
            worksheet.Cells[4, 2] = gpuLoad;
            worksheet.Cells[5, 1] = "Частота работы GPU";
            worksheet.Cells[5, 2] = gpuClockSpeed;
            worksheet.Cells[6, 1] = "Загрузка оперативной памяти";
            worksheet.Cells[6, 2] = ramLoad;

            // Добавление гистограммы
            Excel.ChartObjects chartObjects = (Excel.ChartObjects)worksheet.ChartObjects(Type.Missing);
            Excel.ChartObject chartObject = chartObjects.Add(100, 80, 300, 250);
            Excel.Chart chart = chartObject.Chart;

            Excel.Range dataRange = worksheet.Range["A2", "B6"];
            chart.SetSourceData(dataRange);
            chart.ChartType = Excel.XlChartType.xlColumnClustered;

            // Сохранение файла
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, "StressTestResultsWithHistogram.xlsx");
            workbook.SaveAs(filePath);

            // Закрытие книги и приложения Excel
            workbook.Close();
            excelApp.Quit();

            // Освобождение ресурсов
            System.Runtime.InteropServices.Marshal.ReleaseComObject(chart);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(chartObject);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(chartObjects);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
        }
    }
}
    
    
    

        
    
            
