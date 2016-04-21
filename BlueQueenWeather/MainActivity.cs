using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using BlueQueenWeather.Common;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Generic;
using System.Globalization;

namespace BlueQueenWeather
{
    [Activity(Label = "BlueQueenWeather", Icon = "@drawable/splash")]
    public class MainActivity : Activity
    {
        BlueQueen BQ;
        List<WeatherInfo> WeatherData = new List<WeatherInfo>();

        //test
        Button button;
        TextView tx1;
        TextView tx2;
        //////

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            ServicePointManager.ServerCertificateValidationCallback = validatedCertificate;

            BQ = new BlueQueen(@"http://usafeapi.bluequeen.tk", "v1", "token");

            //WeatherData = BQ.getWeatherData(fromDate: DateTime.Now.ToString());  
            //Przykładowe pobranie dzisiejszych danych (zwraca tablicę WeatherInfo)

            tx1 = FindViewById<TextView>(Resource.Id.textView1);
            tx2 = FindViewById<TextView>(Resource.Id.textView2);
            button = FindViewById<Button>(Resource.Id.button1);
            button.Click += test;

            string text = Intent.GetStringExtra("WeatherData") ?? "[]";
            WeatherData = BQ.deserializeJson<WeatherInfo>(text);
            fillTextboxes();
        }

        private void test(object sender, EventArgs e)
        {
            CultureInfo culture = new CultureInfo("en-US");
            WeatherData = BQ.getWeatherData(fromDate: DateTime.Now.ToString("d", culture)); 
            // inny sposób na datę. (wymaga daty numerycznej)
            fillTextboxes();
        }

        private void fillTextboxes()
        {
            var data = WeatherData.FindLast(x => x.Value > -40);
            tx1.Text = data.Date.ToLongDateString() + " " + data.Date.ToLongTimeString();
            tx2.Text = string.Format("{0}°C", data.Value.ToString());
        }

        private bool validatedCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            string x = "validate cert called for " + certificate.Subject;
            return true;
        }
        
    }
}

