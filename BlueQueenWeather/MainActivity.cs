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
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Xamarin.Android;

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
        PlotView chart = null;
        Button tempChartSw = null;
        Button pressChartSw = null;

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
            if (button != null)
            {
                button.Click += test;
            }

            string text = Intent.GetStringExtra("WeatherData") ?? "[]";
            WeatherData = BQ.deserializeJson<WeatherInfo>(text);
            string text1 = Intent.GetStringExtra("PressureData") ?? "[]";
            PressureData = BQ.deserializeJson<PressureInfo>(text1);
            fillTextboxes();

            chart = FindViewById<PlotView>(Resource.Id.plotview);
            showTempChart();
            tempChartSw = FindViewById<Button>(Resource.Id.tempChart);
            if(tempChartSw != null)
            {
                tempChartSw.Click += delegate 
                {
                    showTempChart();
                };
            }
            pressChartSw = FindViewById<Button>(Resource.Id.pressChart);
            if(pressChartSw != null)
            {
                pressChartSw.Click += delegate
                {
                    showPressureChart();
                };
            }
        }
        

        private void showTempChart()
        {
            if (chart != null)
            {
                var plotModel = new PlotModel { Title = "Temperature chart" };

                plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = 0, Maximum = 24 });
                plotModel.Axes.Add(new LinearAxis { Unit = "°C", Position = AxisPosition.Left, Maximum = WeatherData.Max(x => x.Value) + 2, Minimum = WeatherData.Min(x => x.Value) - 2 });

                var series1 = new LineSeries
                {
                    MarkerType = OxyPlot.MarkerType.Circle,
                    MarkerSize = 2,
                    MarkerStroke = OxyPlot.OxyColors.White
                };

                foreach (var x in WeatherData)
                {
                    double d = (double)x.Date.Hour;
                    double h = ((x.Date.Minute / 60.0));
                    d += h;
                    series1.Points.Add(new DataPoint(d, x.Value));
                }
                plotModel.Series.Add(series1);
                chart.Model = plotModel;
            }
        }

        private void showPressureChart()
        {
            if (chart != null)
            {
                var plotModel = new PlotModel { Title = "Pressure chart" };

                plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = 0, Maximum = 24 });
                plotModel.Axes.Add(new LinearAxis { Unit = "hPa", Position = AxisPosition.Left, Maximum = PressureData.Max(x => x.Pressure) + 0.5, Minimum = PressureData.Min(x => x.Pressure) - 0.5 });

                var series1 = new LineSeries
                {
                    MarkerType = OxyPlot.MarkerType.Circle,
                    MarkerSize = 2,
                    MarkerStroke = OxyPlot.OxyColors.White
                };

                foreach (var x in PressureData)
                {
                    double d = (double)x.Date.Hour;
                    double h = ((x.Date.Minute / 60.0));
                    d += h;
                    series1.Points.Add(new DataPoint(d, x.Pressure));
                }
                plotModel.Series.Add(series1);
                chart.Model = plotModel;
            }
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
            if (tx2 != null)
            {
                var data = WeatherData.FindLast(x => x.Value > -40);
                var press = PressureData.FindLast(x => x.ID > 0);
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

