namespace TemperatureLibrary.Data
{
    public struct TemperatureData
    {
        public TemperatureData()
        {
            this.hardwares = new Dictionary<string, string>();
        }

        public Dictionary<string, string> hardwares { get; set; }
    }
}
