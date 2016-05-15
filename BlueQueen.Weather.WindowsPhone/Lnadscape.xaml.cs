using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace BlueQueen.Weather.WindowsPhone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Lnadscape : Page
    {
        BlueQueenCore BQ;
        List<WeatherInfo> WeatherData = new List<WeatherInfo>();
        List<PressureInfo> PressureData = new List<PressureInfo>();

        public Lnadscape()
        {
            this.InitializeComponent();
            DisplayProperties.OrientationChanged += Orientation;
            this.NavigationCacheMode = NavigationCacheMode.Required;
            BQ = new BlueQueenCore(@"http://usafeapi.bluequeen.tk", "v1", "token");
            CultureInfo culture = new CultureInfo("en-US");
            WeatherData = BQ.getWeatherData(fromDate: DateTime.Now.ToString("d", culture));
            PressureData = BQ.getPressureData(fromDate: DateTime.Now.ToString("d", culture));
            TempChart();     
        }

        private void TempChart()
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
                double h = x.Date.Minute / 60.0;
                d += h;
                series1.Points.Add(new DataPoint(d, x.Value));
            }
            plotModel.Series.Add(series1);
            plot.DataContext = plotModel;
        }

        private void PressureChart()
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
                double h = x.Date.Minute / 60.0;
                d += h;
                series1.Points.Add(new DataPoint(d, x.Pressure));
            }
            plotModel.Series.Add(series1);
            plot.DataContext = plotModel;
            
        }

        public void Orientation(object sender)
        {
            switch (DisplayProperties.CurrentOrientation)
            {
                case DisplayOrientations.Portrait:
                case DisplayOrientations.PortraitFlipped: //goto zwykly
                    Frame.Navigate(typeof(MainPage));
                    break;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void temp_chart_btn_Click(object sender, RoutedEventArgs e)
        {
            TempChart();
        }

        private void pressure_chart_btn_Click(object sender, RoutedEventArgs e)
        {
            PressureChart();
        }
    }
}
