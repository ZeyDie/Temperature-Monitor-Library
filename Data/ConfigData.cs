namespace TemperatureLibrary.Data
{
    public struct ConfigData
    {
        public ConfigData(
            string url,
            string token, 
            int period
        ) : this ()
        {
            this.url = url;
            this.token = token;
            this.period = period;
        }

        public string url { get; set; }
        public string token { get; set; }

        public long period { get; set; }
    }
}
