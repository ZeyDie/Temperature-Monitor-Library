using LibreHardwareMonitor.Hardware;
using System.Text.Json;
using TemperatureLibrary.Sensors;
using static TemperatureLibrary.Sensors.Temperature;

namespace TemperatureLibrary
{
    public class Programm {
        private struct ComputerData
        {
            public Data cpu { get; set; }
            public Data gpu { get; set; }

            public ComputerData(Data cpu, Data gpu)
            {
                this.cpu = cpu;
                this.gpu = gpu;
            }
        }

        public static void Main(String[] args)
        {
            Temperature.Load();

            var cpu = Temperature.GetTemperature(HardwareType.Cpu);

            var gpuIntel = Temperature.GetTemperature(HardwareType.GpuIntel);
            var gpuAMD = Temperature.GetTemperature(HardwareType.GpuAmd);
            var gpuNvidia = Temperature.GetTemperature(HardwareType.GpuNvidia);

            var gpu = new Data();

            if (gpuIntel.avgFloat() != 0)
                gpu = gpuIntel;
            if (gpuAMD.avgFloat() != 0)
                gpu = gpuAMD;
            if (gpuNvidia.avgFloat() != 0)
                gpu = gpuNvidia;

            Console.WriteLine(JsonSerializer.Serialize(new ComputerData(cpu, gpu)));

            Temperature.Close();
        }
    }
}