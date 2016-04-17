using System;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Json;
using System.Security.Cryptography.X509Certificates;

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

        public WeatherInfo[] getData()
        {
            string API = "https://" + apiUrl + "/" + apiVersion + "/weather";
            string GET = "?from=2016-04-05&token=" + apiToken;
            Uri Api = new Uri(API);
            getJson(API,GET);
            throw new NotImplementedException();
        }

        private static void getJson(string url, string GET)
        {
            
            var request = WebRequest.Create(url);
            string text;
            request.ContentType = "application/json; charset=utf-8";
            var response = (HttpWebResponse)request.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }
            text = "";
        }

    }
}