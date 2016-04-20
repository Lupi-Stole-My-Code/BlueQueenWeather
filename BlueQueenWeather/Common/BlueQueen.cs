using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;

namespace BlueQueenWeather.Common
{
    class BlueQueen
    {
        string apiUrl;
        string apiToken;
        string apiVersion;

        public BlueQueen(string apiUrl, string apiVersion, string apiToken)
        {
            this.apiUrl = apiUrl;
            this.apiToken = apiToken;
            this.apiVersion = apiVersion;
        }

        public List<WeatherInfo> getWeatherData(string fromDate = "", string toDate="")
        {
            string API = apiUrl + "/" + apiVersion + "/";
            string GET = "";
            GET += (fromDate != "") ? "from=" + fromDate : "";
            GET += (toDate != "") ? (GET.Length > 0 ? "&to=" : "to=") + fromDate : "";
            string json = getJson(API,"weather",GET);
            var Weather = JsonConvert.DeserializeObject<List<WeatherInfo>>(json);
            return Weather;
        }

        private string getJson(string url,string resource, string GET = "")
        {
            string FQ = (GET.Length > 0) ? resource + "?token={api_token}&" + GET : resource + "?token={api_token}";
            var client = new RestClient(url);
            var request = new RestRequest(FQ, Method.GET);
            request.AddParameter("api_token", apiToken, ParameterType.UrlSegment);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            return content;
        }

    }
}