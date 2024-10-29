namespace TemperatureLibrary.Data
{
    public struct TemperatureData
    {
        public TemperatureData()
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
}
