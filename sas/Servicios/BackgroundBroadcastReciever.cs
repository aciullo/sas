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
using Android.Support.V4.Content;

namespace sas
{
    [BroadcastReceiver(Enabled = true, Permission = "com.google.android.c2dm.permission.SEND")]
    [IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" }, Categories = new string[] { "py.com.futura.sas" })]
    public class BackgroundBroadcastReciever : WakefulBroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
          
            StartWakefulService(context, new Intent(context, typeof(RegistrationIntentService)));
        }
    }
}