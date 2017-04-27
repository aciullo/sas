using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Util;
using Android.Net;
using Android.Media;
using System.Net.Http;
using sas.Core;
using sas.Clases;
using Newtonsoft.Json;
using System.Text;
namespace sas
{
   
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
   
    public class MyGcmListenerService : GcmListenerService
    {
        public override void OnMessageReceived(string from, Bundle data)
        {
            var message = data.GetString("message");
            Log.Debug("MyGcmListenerService", "From:    " + from);
            Log.Debug("MyGcmListenerService", "Message: " + message);
            SendNotification(message);
           
        }

        async void SendNotification(string message)
        {
            var intent = new Intent(this, typeof(Servicios));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);


            //set defaults toma el sonido por defecto en el dispositivo
            // se puede definir el tipo de alarma con el siguiente comando
            // to play the alarm sound with your notification
            //builder.SetSound (RingtoneManager.GetDefaultUri(RingtoneType.Alarm));
            //  Alternatively, you can use the system default ringtone sound for your notification:
            //builder.SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Ringtone));

            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.ic_stat_ic_notification)
                .SetContentTitle("SAS Futura")
                .SetContentText(message)
                .SetAutoCancel(true)
                .SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate)
                .SetContentIntent(pendingIntent);

            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(0, notificationBuilder.Build());

            AlarmManager manager = (AlarmManager)GetSystemService(Context.AlarmService);
            manager.Set(AlarmType.Rtc, System.DateTime.UtcNow.Millisecond + 5000, pendingIntent);
            // manager.set(AlarmManager.RTC, System.currentTimeMillis() + 5000, yourIntent);

            //mandar un log para cuando recibe la noficacion
            string result;
            var session = new UserSessionManager(this);
            var IPCONN = session.getAccessConn();
            var movil = session.getAccessIdmovil();
            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            System.Net.Http.HttpResponseMessage response;
            client.BaseAddress = new System.Uri(IPCONN);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + session.getAccessToken());
            string url;
            var servicio = new ServicioItem();
            servicio.AuditUsuario = "Sistema";
            servicio.Nombre = "Recibe Solicitud movil " + movil;
            var jsonResquest = JsonConvert.SerializeObject(servicio);
            var content = new StringContent(jsonResquest, System.Text.Encoding.UTF8, "text/json");
            url = string.Format("api/ProcesoEstadoServiciosApi");
            response = await client.PutAsync(url, content);
            result = response.Content.ReadAsStringAsync().Result;


        }
    }
}