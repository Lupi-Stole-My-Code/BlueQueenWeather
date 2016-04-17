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

namespace BlueQueenWeather
{
    [Activity(Label = "BlueQueenWeather", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ServicePointManager.ServerCertificateValidationCallback = validatedCertificate;

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            
            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };

            BlueQueen BQ = new BlueQueen("api.bluequeen.tk", "v1", "token");
            var c = BQ.getData();
            c = null;
        }

        private bool validatedCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            string x = "validate cert called for " + certificate.Subject;
            return true;
        }
    }
}

