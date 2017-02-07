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

[assembly: Permission(Name = "py.com.futura.sas.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
[assembly: UsesPermission(Name = "py.com.futura.sas.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]

namespace sas
{
    [BroadcastReceiver(Permission = "com.google.android.c2dm.permission.SEND")]
    [IntentFilter(new string[] { "com.google.android.c2dm.intent.RECEIVE" }, Categories = new string[] { "py.com.futura.sas" })]
    [IntentFilter(new string[] { "com.google.android.c2dm.intent.REGISTRATION" }, Categories = new string[] { "py.com.futura.sas" })]
    [IntentFilter(new string[] { "com.google.android.gcm.intent.RETRY" }, Categories = new string[] { "py.com.futura.sas" })]
    public class GcmBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            PowerManager.WakeLock sWakeLock;
            var pm = PowerManager.FromContext(context);
            sWakeLock = pm.NewWakeLock(WakeLockFlags.Partial, "GCM Broadcast Reciever Tag");
            sWakeLock.Acquire();

            //handle the notification here
            var message = intent.GetStringExtra("message");
          
            SendNotification(message, context);

            sWakeLock.Release();

        }
        void SendNotification(string message, Context context)
        {
            var intent = new Intent(context, typeof(Servicios));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder(context)
                .SetSmallIcon(Resource.Drawable.ic_stat_ic_notification)
                .SetContentTitle("SAS Futura")
                .SetContentText(message)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            var notificationManager = (NotificationManager)context.GetSystemService (Context.NotificationService);
            notificationManager.Notify(0, notificationBuilder.Build());

            AlarmManager manager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            manager.Set(AlarmType.RtcWakeup, System.DateTime.UtcNow.Millisecond + 5000, pendingIntent);
            // manager.set(AlarmManager.RTC, System.currentTimeMillis() + 5000, yourIntent);

        }
    }
}