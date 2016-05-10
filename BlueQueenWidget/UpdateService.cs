using System;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Widget;
using BlueQueen;
using System.Globalization;
using Android.Runtime;
using Android.Content.Res;

namespace SimpleWidget
{
    [Service]
    public class UpdateService : Service
    {

        double timer = 1.5; // time interval in minutes
        ComponentName thisWidget = null;
        AppWidgetManager manager = null;
        public override void OnStart(Intent intent, int startId)
        {
            thisWidget = new ComponentName(this, Java.Lang.Class.FromType(typeof(WordWidget)).Name);
            manager = AppWidgetManager.GetInstance(this);
            T_Elapsed(null, null);

            System.Timers.Timer t = new System.Timers.Timer();
            t.Elapsed += T_Elapsed;
            t.Interval = (int)(timer * 1000 * 60);
            t.Start();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (thisWidget != null && manager != null)
            {
                T_Elapsed(null, null);
            }
            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            if (thisWidget != null && manager != null)
            {
                T_Elapsed(null, null);
            }
            base.OnConfigurationChanged(newConfig);
        }

        private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            RemoteViews updateViews = buildUpdate(this);
            manager.UpdateAppWidget(thisWidget, updateViews);
        }

        public override IBinder OnBind(Intent intent)
        {
            // We don't need to bind to this service
            return null;
        }

        public RemoteViews buildUpdate(Context context)
        {
            var BQ = new BlueQueenCore(@"http://usafeapi.bluequeen.tk", "v1", "token");
            CultureInfo culture = new CultureInfo("en-US");
            var WeatherData = BQ.getWeatherData(fromDate: DateTime.Now.ToString("d", culture));
            var entry = WeatherData.FindLast(x => x.ID > 0);

            var PressureData = BQ.getPressureData(fromDate: DateTime.Now.ToString("d", culture));
            var entry2 = PressureData.FindLast(x => x.ID > 0);

            // Build an update that holds the updated widget contents
            var updateViews = new RemoteViews(context.PackageName, Resource.Layout.widget_word);

            updateViews.SetTextViewText(Resource.Id.blog_title, entry.Date.ToLongDateString() + " " + entry.Date.ToLongTimeString());
            updateViews.SetTextViewText(Resource.Id.creator, "Wejherowo | " + entry.Value.ToString() + "°C | " + entry2.Pressure.ToString() + " hPa");

            return updateViews;
        }
    }
}
