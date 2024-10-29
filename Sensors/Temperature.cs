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
                                var value = sensor.Value.GetValueOrDefault();

                                if (name.Contains("Max"))
                                    data.max = value.ToString("0");
                                else if (name.Contains("Average") || name.Contains("GPU"))
                                    data.avg = value.ToString("0");
                                else if (data.minFloat() > value || data.minFloat() == 0)
                                    data.min = value.ToString("0");
                            }
                    foreach (var sensor in hardware.Sensors)
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            var name = sensor.Name;
                            var value = sensor.Value.GetValueOrDefault();

                            if (name.Contains("Max"))
                                data.max = value.ToString("0");
                            else if (name.Contains("Average") || name.Contains("GPU"))
                                data.avg = value.ToString("0");
                            else if (data.minFloat() > value || data.minFloat() == 0)
                                data.min = value.ToString("0");
                        }

                    if (data.avgFloat() == 0)
                    {
                        float max = data.maxFloat();
                        float min = data.minFloat();

                        float avg = (float)((max + min) / 2.0);

                        data.avg = avg.ToString("0");
                    }

                    return data;
                }

            return data;
        }
    }
}
