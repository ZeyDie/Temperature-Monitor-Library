using LibreHardwareMonitor.Hardware;
using TemperatureLibrary.Data;

namespace TemperatureLibrary.Sensors
{
    public class Temperature
    {
        public static TemperatureData GetTemperature(IComputer computer, HardwareType type)
        {
            var data = new TemperatureData();

            if (computer == null) return data;

            foreach (var hardware in computer.Hardware)
                if (hardware.HardwareType == type)
                {
                    hardware.Update();

                    foreach (var subHardware in hardware.SubHardware)
                        foreach (var sensor in subHardware.Sensors)
                            if (sensor.SensorType == SensorType.Temperature)
                            {
                                subHardware.Update();

                                var name = sensor.Name;
                                var value = sensor.Value.GetValueOrDefault().ToString("0");

                                data.hardwares.Add(name, value);
                            }
                    foreach (var sensor in hardware.Sensors)
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            var name = sensor.Name;
                            var value = sensor.Value.GetValueOrDefault().ToString("0");

                            data.hardwares.Add(name, value);
                        }

                    return data;
                }

            return data;
        }
    }
}
