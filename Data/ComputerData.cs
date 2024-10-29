namespace TemperatureLibrary.Data
{   
    public struct ComputerData
    {
        public ComputerData(
            TemperatureData cpu,
            TemperatureData gpu
        ) : this()
        {
            this.token = null;
            this.cpu = cpu;
            this.gpu = gpu;
        }

        public ComputerData(
            string token,
            TemperatureData cpu,
            TemperatureData gpu
        ) : this()
        {
            this.token = token;
            this.cpu = cpu;
            this.gpu = gpu;
        }

        public string token { get; set; }

        public TemperatureData cpu { get; set; }
        public TemperatureData gpu { get; set; }
    }
}
