using LibreHardwareMonitor.Hardware;
using System.Numerics;
using TemperatureLibrary.LibsAPI;
using static TemperatureLibrary.LibsAPI.LHMApi;

namespace TemperatureLibrary.Sensors
{
    public class Temperature
    {
        public struct Data
        {
            public Data()
            {
                this.avg = "0";
                this.min = "0";
                this.max = "0";
            }

            public string avg { get; set; }
            public string min { get; set; }
            public string max { get; set; }

            public float avgFloat()
            {
                return float.Parse(avg);
            }

            public float minFloat()
            {
                return float.Parse(min);
            }

            public float maxFloat()
            {
                return float.Parse(max);
            }

            public override string ToString()
            {
                return string.Format(
                    "avg: {0}; min: {1}; max: {2}",
                    this.avg,
                    this.min,
                    this.max
                );
            }
        }

        private static Computer computer = new Computer();

        public static void Load()
        {
            computer.Open();
            computer.IsCpuEnabled = true;
            computer.IsGpuEnabled = true;
            //computer.IsMotherboardEnabled = true;
            computer.Accept(new UpdateVisitor());
        }

        public static Data GetTemperature(HardwareType type)
        {
            var data = new Data();

            foreach (var hardware in computer.Hardware)
            {
                if (hardware.HardwareType == type)
                {
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            var name = sensor.Name;
                            var value = sensor.Value;

                            if (name.Contains("Max"))
                                data.max = value.GetValueOrDefault().ToString("0");
                            else if (name.Contains("Average") || name.Contains("GPU"))
                                data.avg = value.GetValueOrDefault().ToString("0");
                            else if (data.minFloat() > value || data.minFloat() == 0)
                                data.min = value.GetValueOrDefault().ToString("0");
                        }
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
            }

            return data;
        }

        public static void Close()
        {
            computer.Close();
        }
    }
}
