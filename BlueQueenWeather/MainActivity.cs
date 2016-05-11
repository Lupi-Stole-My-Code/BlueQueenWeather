using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Generic;
using System.Globalization;
using BlueQueen;
using Android.Graphics.Drawables;
using System.Linq;

namespace BlueQueenWeather
{
    [Activity(Label = "BlueQueen Weather")]
    public class MainActivity : Activity
    {
        BlueQueenCore BQ;
        List<WeatherInfo> WeatherData = new List<WeatherInfo>();
        List<PressureInfo> PressureData = new List<PressureInfo>();

        //test
        Button button;
        TextView tx1 = null;
        TextView tx2 = null;
        TextView pressureTxt = null;
        TextView averagepressureTxt = null;
        TextView maximaltempTxt = null;
        TextView minimaltempTxt = null;
        TextView averageTemperatureTxt = null;

        //////

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            if ((int)Android.OS.Build.VERSION.SdkInt >= 21)
            {
                ActionBar.SetIcon(
                  new ColorDrawable(Resources.GetColor(Android.Resource.Color.Transparent)));
            }
            
            ServicePointManager.ServerCertificateValidationCallback = validatedCertificate;

            BQ = new BlueQueenCore(@"http://usafeapi.bluequeen.tk", "v1", "token");

            //Portrait
            //tx1 = FindViewById<TextView>(Resource.Id.textView1);
            tx2 = FindViewById<TextView>(Resource.Id.textView2);
            averagepressureTxt = FindViewById<TextView>(Resource.Id.averagepress);
            pressureTxt = FindViewById<TextView>(Resource.Id.cisnienie);
            maximaltempTxt = FindViewById<TextView>(Resource.Id.maximaltemp);
            minimaltempTxt = FindViewById<TextView>(Resource.Id.minimaltemp);
            averageTemperatureTxt = FindViewById<TextView>(Resource.Id.averagetemp);
            button = FindViewById<Button>(Resource.Id.button1);
            button.Click += test;

            string text = Intent.GetStringExtra("WeatherData") ?? "[]";
            WeatherData = BQ.deserializeJson<WeatherInfo>(text);
            string text1 = Intent.GetStringExtra("PressureData") ?? "[]";
            PressureData = BQ.deserializeJson<PressureInfo>(text1);
            fillTextboxes();
        }

        private void test(object sender, EventArgs e)
        {
            CultureInfo culture = new CultureInfo("en-US");
            WeatherData = BQ.getWeatherData(fromDate: DateTime.Now.ToString("d", culture));
            PressureData = BQ.getPressureData(fromDate: DateTime.Now.ToString("d", culture));
            // inny sposób na datę. (wymaga daty numerycznej)
            fillTextboxes();
            Toast toast = Toast.MakeText(this, "Successfully updated", ToastLength.Short);
            toast.Show();
        }

        private void fillTextboxes()
        {
            var data = WeatherData.FindLast(x => x.Value > -40);
            var press = PressureData.FindLast(x => x.ID > 0);
            if (tx2 != null)
            {
                //tx1.Text = data.Date.ToLongDateString() + " " + data.Date.ToLongTimeString();
                tx2.Text = string.Format("{0}°C", data.Value.ToString());

                pressureTxt.Text = string.Format("{0} hPa", press.Pressure.ToString());
                averagepressureTxt.Text = string.Format("{0:0.00} hPa", PressureData.Average(x => x.Pressure));
                averageTemperatureTxt.Text = string.Format("{0:0.00}°C", WeatherData.Average(x => x.Value));
                minimaltempTxt.Text = string.Format("{0:0.00}°C", WeatherData.Min(x => x.Value));
                maximaltempTxt.Text = string.Format("{0:0.00}°C", WeatherData.Max(x => x.Value));
            }
            else
            {
                Toast toast = Toast.MakeText(this, "Nothing to refresh : Landscape mode", ToastLength.Short);
            }
        }

        private bool validatedCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            string x = "validate cert called for " + certificate.Subject;
            return true;
        }
        
    }
}

