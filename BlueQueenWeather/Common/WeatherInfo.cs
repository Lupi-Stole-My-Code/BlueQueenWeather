using System;
using Newtonsoft.Json;

namespace BlueQueenWeather.Common
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
        public string Date { get; set; }
        [JsonProperty("Value")]
        public string Value { get; set; }
    }
}