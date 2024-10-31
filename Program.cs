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

        private static string defaultProtocol = "http://";
        private static int defaultPort = 8100;
        private static string defaultApi = "/api/v2/temperature";

        public static void Main(String[] args)
        {
            if (!File.Exists(configPath))
            {
                Console.WriteLine("Type token");
                var token = Console.ReadLine();

                Console.WriteLine("Type server address or type enter to skip");
                var address = Console.ReadLine();

                Console.WriteLine("Type server port or type enter to skip");
                var port = Console.ReadLine();

                if (port.Length != 0)
                    defaultPort = int.Parse(port);

                if (address.Length == 0)
                    address = defaultProtocol + "localhost" + ":" + defaultPort + defaultApi;
                else
                {
                    bool hasProtocol = address.StartsWith(defaultProtocol) || address.StartsWith("https://");
                    bool hasPort = address.Contains(':');
                    bool hasUrl = address.EndsWith(defaultApi);

                    if (!hasProtocol)
                        address = defaultProtocol + address;
                    if (!hasPort)
                        address = address + ":" + defaultPort;
                    if (!hasUrl)
                        address = address + defaultApi;
                }

                using var streamWriter = new StreamWriter(configPath, true);

                streamWriter.Write(
                    JsonSerializer.Serialize(
                        new ConfigData(
                            address,
                            token,
                            5 * 1000
                        )
                    )
                );
            }

            if (!args.ToList().Contains("-nohide"))
                consoleHider.HideWindow();

            startupManagerWindows.Startup = startupManagerWindows.IsAvailable;

            var configData = JsonSerializer.Deserialize<ConfigData>(File.ReadAllText(configPath));

            temperatureLibrary.Launch(configData);
        }
    }
}