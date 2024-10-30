using LibreHardwareMonitor.Hardware;
using static TemperatureLibrary.LibsAPI.LHMApi;
using System.Timers;
using TemperatureLibrary.Data;
using System.Text.Json;
using TemperatureLibrary.LibsAPI;
using TemperatureLibrary.Sensors;

namespace TemperatureLibrary
{
    public class TemperatureLibrary
    {
        public void Launch(ConfigData configData)
        {
            var computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true
            };

            computer.Open();
            computer.Accept(new UpdateVisitor());

            var timer = new System.Timers.Timer(configData.period);

            timer.Elapsed += new ElapsedEventHandler(
                (source, elapsedEventArgs) => {
                    var computerData = GetComputerData(computer);

                    computerData.token = configData.token;

                    var json = JsonSerializer.Serialize(computerData);

                    Console.WriteLine(json);

                    LHMApi.SendComputerData(configData.url, json);
                }
            );
            timer.Start();

            Console.WriteLine("Press the Enter key to exit the program.");
            Console.ReadLine();

            computer.Close();
        }

        public static ComputerData GetComputerData(Computer computer)
        {
            var cpu = Temperature.GetTemperature(computer, HardwareType.Cpu);

            var gpuIntel = Temperature.GetTemperature(computer, HardwareType.GpuIntel);
            var gpuAMD = Temperature.GetTemperature(computer, HardwareType.GpuAmd);
            var gpuNvidia = Temperature.GetTemperature(computer, HardwareType.GpuNvidia);

            var gpu = new TemperatureData();

            if (gpuIntel.hardwares.Count > 0)
                gpu = gpuIntel;
            if (gpuAMD.hardwares.Count > 0)
                gpu = gpuAMD;
            if (gpuNvidia.hardwares.Count > 0)
                gpu = gpuNvidia;

            return new ComputerData(cpu, gpu);
        }
    }
}
