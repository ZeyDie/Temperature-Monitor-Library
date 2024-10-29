using LibreHardwareMonitor.Hardware;
using System.Text;

namespace TemperatureLibrary.LibsAPI
{
    public class LHMApi
    {
        public class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }

            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();

                foreach (var subHardware in hardware.SubHardware)
                    subHardware.Accept(this);
            }

            public void VisitSensor(ISensor sensor) { 
               
            }

            public void VisitParameter(IParameter parameter) { }
        }
    
        public static async void SendComputerData(string url, string json)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    await client.PostAsync(url, content);
                }
                catch (HttpRequestException exception)
                {
                    Console.WriteLine("Ошибка при отправке запроса: " + exception.Message);
                }
            }
        }
    }
}
