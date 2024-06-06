using System;
using System.Collections.Generic;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using DocumentFormat.OpenXml.ExtendedProperties;
using OpenHardwareMonitor.Hardware;
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
            SaveResultsToWord();
            Test test = new Test();
            test.RunAllStressTests();
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
            // Дополнительный код для измерения температуры CPU, GPU и других компонентов
            // Можете добавить здесь код для измерения температуры
        }

       


        private void SaveResultsToWord()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, "StressTestResults.docx");

            object fileName = filePath;
            object missing = System.Reflection.Missing.Value;

            // Записываем результаты в документ Word
            doc.Content.Text += "Результаты стресс-тестирования:\n\n";
            doc.Content.Text += $"Температура процессора: {cpuTemperature} °C\n";

            doc.SaveAs2(ref fileName, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
            doc.Close();
            wordApp.Quit();

            Console.WriteLine("Результаты стресс-тестирования успешно записаны в документ Word на рабочем столе.");
        }
    }
}