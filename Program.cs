using System.Text.Json;
using TemperatureLibrary.Data;
using System.Reflection;
using TemperatureLibrary.LibsAPI;

namespace TemperatureLibrary
{
    public class Programm {
        public static string name = "Temperature Library";
        public static string location = Assembly.GetExecutingAssembly().Location;

        private static string configPath = "config.json";

        private static ConsoleHider consoleHider = new();
        private static StartupManagerWindows startupManagerWindows = new();

        private static TemperatureLibrary temperatureLibrary = new();

        public static void Main(String[] args)
        {
            if (!args.ToList().Contains("-nohide"))
                consoleHider.HideWindow();

            startupManagerWindows.Startup = startupManagerWindows.IsAvailable;

            if (!File.Exists(configPath))
            {
                using var streamWriter = new StreamWriter(configPath, true);

                streamWriter.Write(
                    JsonSerializer.Serialize(
                        new ConfigData(
                            "http://localhost:8100/api/v2/temperature",
                            "typeHereTokenOfBot",
                            5 * 1000
                        )
                    )
                );
            }

            var configData = JsonSerializer.Deserialize<ConfigData>(File.ReadAllText(configPath));

            temperatureLibrary.Launch(configData);
        }
    }
}