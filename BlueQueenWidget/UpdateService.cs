/*
 * Copyright (C) 2009 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Widget;
using BlueQueen;
using System.Globalization;

namespace SimpleWidget
{
    [Service]
    public class UpdateService : Service
    {
        public override void OnStart(Intent intent, int startId)
        {
            // Build the widget update for today
            RemoteViews updateViews = buildUpdate(this);

            // Push update for this widget to the home screen
            ComponentName thisWidget = new ComponentName(this, Java.Lang.Class.FromType(typeof(WordWidget)).Name);
            AppWidgetManager manager = AppWidgetManager.GetInstance(this);
            manager.UpdateAppWidget(thisWidget, updateViews);
        }

        public override IBinder OnBind(Intent intent)
        {
            // We don't need to bind to this service
            return null;
        }


        // Build a widget update to show the current Wiktionary
        // "Word of the day." Will block until the online API returns.
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
            updateViews.SetTextViewText(Resource.Id.creator, entry.Value.ToString() + "°C | " + entry2.Pressure.ToString() + "hPa");

            // When user clicks on widget, launch to Wiktionary definition page
            //if (!string.IsNullOrEmpty (entry.Link)) {
            //	Intent defineIntent = new Intent (Intent.ActionView, Android.Net.Uri.Parse (entry.Link));

            //	PendingIntent pendingIntent = PendingIntent.GetActivity (context, 0, defineIntent, 0);
            //	updateViews.SetOnClickPendingIntent (Resource.Id.widget, pendingIntent);
            //}

            return updateViews;
        }
    }
}
