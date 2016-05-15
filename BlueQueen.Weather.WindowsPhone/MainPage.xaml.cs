using System;
using System.IO;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Globalization;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using System.Linq;
using BlueQueen;
using Windows.UI.Popups;
using Windows.Graphics.Display;

namespace BlueQueen.Weather.WindowsPhone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        BlueQueenCore BQ;
        List<WeatherInfo> WeatherData = new List<WeatherInfo>();
        List<PressureInfo> PressureData = new List<PressureInfo>();


        public MainPage()
        {
            this.InitializeComponent();
            DisplayProperties.OrientationChanged += Orientation;
            
            BQ = new BlueQueenCore(@"http://usafeapi.bluequeen.tk", "v1", "token");
            CultureInfo culture = new CultureInfo("en-US");
            WeatherData = BQ.getWeatherData(fromDate: DateTime.Now.ToString("d", culture));
            PressureData = BQ.getPressureData(fromDate: DateTime.Now.ToString("d", culture));

            //var test = WeatherData;
            fillTextBlock();

            //tile
            var tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText01);
            var tileImage = tileXml.GetElementsByTagName("image")[0] as XmlElement;
            tileImage.SetAttribute("src", "ms-appx:///Assets/Square71x71Logo.scale-100.png");

            var tileText = tileXml.GetElementsByTagName("text");
            WeatherInfo lastTemp = WeatherData.Last(x => x.ID > 0);
            PressureInfo lastPress = PressureData.Last(x => x.ID > 0);
            (tileText[0] as XmlElement).InnerText = "Wejherowo";
            (tileText[1] as XmlElement).InnerText = lastTemp.Date.ToString();
            (tileText[2] as XmlElement).InnerText = string.Format("{0}°C", lastTemp.Value.ToString());
            (tileText[3] as XmlElement).InnerText = string.Format("{0} hPa", lastPress.Pressure.ToString());
            var tileNotification = new TileNotification(tileXml);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
            // eoTile
        }

        public void Orientation(object sender)
        {
            switch (DisplayProperties.CurrentOrientation)
            {
                case DisplayOrientations.Landscape:
                case DisplayOrientations.LandscapeFlipped:
                    Frame.Navigate(typeof(Lnadscape));
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
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }
        public void fillTextBlock()
        {
            var data = WeatherData.FindLast(x => x.Value > -40);
            currentemp_btn.Content = string.Format("{0}°C", data.Value.ToString());
            var press = PressureData.FindLast(x => x.ID > 0);
            currentpress_btn.Content = string.Format("{0} hPa", press.Pressure.ToString());
            averagepress_btn.Content = string.Format("{0:0.00} hPa", PressureData.Average(x => x.Pressure));
            averagetemp_btn.Content = string.Format("{0:0.00}°C", WeatherData.Average(x => x.Value));
            minimaltemp_btn.Content = string.Format("{0:0.00}°C", WeatherData.Min(x => x.Value));
            maximaltemp_btn.Content = string.Format("{0:0.00}°C", WeatherData.Max(x => x.Value));
        }
    

        private void refreshData(object sender, RoutedEventArgs e)
        {
            CultureInfo culture = new CultureInfo("en-US");
            WeatherData = BQ.getWeatherData(fromDate: DateTime.Now.ToString("d", culture));
            fillTextBlock();
            ShowToastNotification("New Data Recived");

        }
        private void ShowToastNotification(String message)
        {
            //http://stackoverflow.com/questions/23498187/display-local-toast-notification
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            // Set Text
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(message));

            // toast duration
            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "short");

            // toast navigation
            var toastNavigationUriString = "#/MainPage.xaml?param1=12345";
            var toastElement = ((XmlElement)toastXml.SelectSingleNode("/toast"));
            toastElement.SetAttribute("launch", toastNavigationUriString);

            // Create the toast notification based on the XML content you've specified.
            ToastNotification toast = new ToastNotification(toastXml);

            // Send your toast notification.
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private void refresh_btn_Click(object sender, RoutedEventArgs e)
        {
            refreshData(sender, e);
        }

        private void currentemp_btn_Click(object sender, RoutedEventArgs e)
        {
            var min = WeatherData.FindLast(x => x.ID > 0);
            string last = string.Format("{0:0.00}°C", min.Value);
            
            new MessageDialog("Measured on:\n" + min.Date.ToString()).ShowAsync();
        }

        private void minimaltemp_btn_Click(object sender, RoutedEventArgs e)
        {
            double a = WeatherData.Min(x => x.Value);
            var min = WeatherData.First(x => x.Value.ToString() == a.ToString());
            
            new MessageDialog(min.Date.ToString() + "\nToday's MIN : " + min.Value.ToString() + "°C\nMeasured on " + min.Date.ToString()).ShowAsync();

        }

        private void averagetemp_btn_Click(object sender, RoutedEventArgs e)
        {
            var a = WeatherData.Average(x => x.Value);
            string avg = string.Format("{0:0.00}°C", a);
            var min = WeatherData.FindLast(x => x.ID > 0);
            string last = string.Format("{0:0.00}°C", min.Value);
            string diff = string.Format("{0:0.00}°C", Math.Abs(a - min.Value));
            new MessageDialog(min.Date.ToString() + "\nToday's AVG : " + avg + "\nCurrent : " + last + "\nDiff : " + diff).ShowAsync();

        }

        private void maximaltemp_btn_Click(object sender, RoutedEventArgs e)
        {
            double a = WeatherData.Max(x => x.Value);
            var min = WeatherData.First(x => x.Value.ToString() == a.ToString());
            
            new MessageDialog(min.Date.ToString() + "\nToday's MAX : " + min.Value.ToString() + "°C\nMeasured on " + min.Date.ToString()).ShowAsync();

        }

        private void currentpress_btn_Click(object sender, RoutedEventArgs e)
        {
            var min = PressureData.FindLast(x => x.ID > 0);
            string last = string.Format("{0:0.00} hPa", min.Pressure);
           
            new MessageDialog("Measured on:\n" + min.Date.ToString()).ShowAsync();

        }

        private void averagepress_btn_Click(object sender, RoutedEventArgs e)
        {
            var a = PressureData.Average(x => x.Pressure);
            string avg = string.Format("{0:0.00} hPa", a);
            var min = PressureData.FindLast(x => x.ID > 0);
            string last = string.Format("{0:0.00} hPa", min.Pressure);
            string diff = string.Format("{0:0.00} hPa", Math.Abs(a - min.Pressure));

            new MessageDialog(min.Date.ToString() + "\nToday's AVG : " + avg + "\nCurrent : " + last + "\nDiff : " + diff).ShowAsync();

        }
    }
}
