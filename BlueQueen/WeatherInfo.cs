using System;
using Newtonsoft.Json;

namespace BlueQueen
{
    public class WeatherInfos
    {
        public WeatherInfo[] Property1 { get; set; }
    }

    public class WeatherInfo
    {
        [JsonProperty("ID")]
        public int ID { get; set; }
        [JsonProperty("Date")]
        public DateTime Date { get; set; }
        [JsonProperty("Value")]
        public double Value { get; set; }
    }
}