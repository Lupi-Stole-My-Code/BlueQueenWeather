
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using System.Threading.Tasks;
using System.Globalization;
using BlueQueen;

namespace BlueQueenWeather
{
	[Activity(Theme = "@style/MyTheme.Splash", Label = "BlueQueen Weather", MainLauncher = true, NoHistory = true, Icon = "@drawable/splash")]
	public class Splash : Activity
	{
		static readonly string TAG = "X:" + typeof(Splash).Name;
        BlueQueenCore BQ;
        string WeatherJson;
        TextView loadingInfo;

        protected override void OnCreate(Bundle savedInstanceState)
		{
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);
			Log.Debug(TAG, "Splash.OnCreate");
            SetContentView(Resource.Layout.splash);
            loadingInfo = FindViewById<TextView>(Resource.Id.loadingState);
            BQ = new BlueQueenCore(@"http://usafeapi.bluequeen.tk", "v1", "token");
        }

		protected override void OnResume()
		{
			base.OnResume();

			Task startupWork = new Task(() => {
				Log.Debug(TAG, "Performing some startup work that takes a bit of time.");
				//Task.Delay(5000);  // Simulate a bit of startup work.
                CultureInfo culture = new CultureInfo("en-US");
                loadingInfo.Text = "Pobieranie danych";
                WeatherJson = BQ.getWeatherDataJsonOnly(fromDate: DateTime.Now.ToString("d", culture));
                Log.Debug(TAG, "Working in the background - important stuff.");
			});

			startupWork.ContinueWith(t => {
				Log.Debug(TAG, "Work is finished - start Activity1.");
                var MainActiv = new Intent(Application.Context, typeof(MainActivity));
                MainActiv.PutExtra("WeatherData", WeatherJson);
                loadingInfo.Text = "Ładowanie zakończone";
                StartActivity(MainActiv);
			}, TaskScheduler.FromCurrentSynchronizationContext());

			startupWork.Start();
		}
	}
}

