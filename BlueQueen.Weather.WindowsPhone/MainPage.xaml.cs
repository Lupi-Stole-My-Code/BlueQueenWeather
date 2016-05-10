using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BlueQueen;
using System.Net;
//using System.Security.Cryptography.X509Certificates;
//using System.Net.Security;
using System.Collections.Generic;
using System.Globalization;
using Windows.Phone.Notification;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace BlueQueen.Weather.WindowsPhone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        BlueQueenCore BQ;
        List<WeatherInfo> WeatherData = new List<WeatherInfo>();
        
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            var tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText01);
            var tileImage = tileXml.GetElementsByTagName("image")[0] as XmlElement;
            tileImage.SetAttribute("src", "ms-appx:///Assets/Square71x71Logo.scale-100.png");

            var tileText = tileXml.GetElementsByTagName("text");
            (tileText[0] as XmlElement).InnerText = "Wszystko";
            (tileText[1] as XmlElement).InnerText = "Muszę";
            (tileText[2] as XmlElement).InnerText = "Robić";
            (tileText[3] as XmlElement).InnerText = "Sam";

            var tileNotification = new TileNotification(tileXml);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);

            BQ = new BlueQueenCore(@"http://usafeapi.bluequeen.tk", "v1", "token");
            CultureInfo culture = new CultureInfo("en-US");
            WeatherData = BQ.getWeatherData(fromDate: DateTime.Now.ToString("d", culture));
            //var test = WeatherData;
            fillTextBlock();
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
            textBlock.Text = data.Date.ToString() + " " + string.Format("{0}°C", data.Value.ToString()); ;
        }
    

        private void refreshData(object sender, RoutedEventArgs e)
        {
            CultureInfo culture = new CultureInfo("en-US");
            WeatherData = BQ.getWeatherData(fromDate: DateTime.Now.ToString("d", culture));
            fillTextBlock();
            ShowToastNotification("abc");

        }
        private void ShowToastNotification(String message)
        {
            //http://stackoverflow.com/questions/23498187/display-local-toast-notification
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            // Set Text
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(message));

            // Set image
            // Images must be less than 200 KB in size and smaller than 1024 x 1024 pixels.
            //XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
            //((XmlElement)toastImageAttributes[0]).SetAttribute("src", "ms-appx:///Images/logo-80px-80px.png");
            //((XmlElement)toastImageAttributes[0]).SetAttribute("alt", "logo");

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
    }
}
