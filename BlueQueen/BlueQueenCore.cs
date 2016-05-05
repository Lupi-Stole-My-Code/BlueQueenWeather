using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlueQueen
{
    public class BlueQueenCore
    {
        string apiUrl;
        string apiToken;
        string apiVersion;

        public BlueQueenCore(string apiUrl, string apiVersion, string apiToken)
        {
            this.apiUrl = apiUrl;
            this.apiToken = apiToken;
            this.apiVersion = apiVersion;
        }

        public List<WeatherInfo> getWeatherData(string fromDate = "", string toDate="")
        {
            string json = getWeatherDataJsonOnly(fromDate, toDate);
            var Weather = JsonConvert.DeserializeObject<List<WeatherInfo>>(json);
            return Weather;
        }

		public List<PressureInfo> getPressureData(string fromDate = "", string toDate="")
		{
			string json = getPressureDataJsonOnly(fromDate, toDate);
			var Weather = JsonConvert.DeserializeObject<List<PressureInfo>>(json);
			return Weather;
		}

        public string getWeatherDataJsonOnly(string fromDate = "", string toDate="")
        {
            string API = apiUrl + "/" + apiVersion + "/";
            string GET = "";
            GET += (fromDate != "") ? "from=" + fromDate : "";
            GET += (toDate != "") ? (GET.Length > 0 ? "&to=" : "to=") + fromDate : "";
            return getJson(API, "weather", GET);
        }

		public string getPressureDataJsonOnly(string fromDate = "", string toDate="")
		{
			string API = apiUrl + "/" + apiVersion + "/";
			string GET = "";
			GET += (fromDate != "") ? "from=" + fromDate : "";
			GET += (toDate != "") ? (GET.Length > 0 ? "&to=" : "to=") + fromDate : "";
			return getJson(API, "pressure", GET);
		}

        private string getJson(string url,string resource, string GET = "")
        {
            string FQ = (GET.Length > 0) ? resource + "?token={api_token}&" + GET : resource + "?token={api_token}";
            var client = new HttpClient();
            client.BaseAddress = new Uri(url);
            var response = client.GetAsync(FQ).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public List<T> deserializeJson<T>(string Json)
        {
            return JsonConvert.DeserializeObject<List<T>>(Json);
        }

    }
}