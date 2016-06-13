using System;
using Android.App;
using Android.Util;

using Android.Content;
using Android.Widget;
using Android.OS;
using System.Net.Http;
using System.Timers;
using System.Threading;
using static sas.DemoService;
using sas.Clases;
using sas.Core;
using Android.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

namespace sas
{
    [Service]
    [IntentFilter(new String[] { "com.sas.searchpending" })]
    public class BuscarServiciosService : Service
    {
        BuscarServiciosServiceBinder binder;
        IList<ServicioItem> servicios;
        ServicioItem servicioDetalle;
        private System.Timers.Timer timer;
        private System.Timers.Timer timerSinc;
        private int counter = 0, incrementby = 1;
        UserSessionManager session;
        string IPCONN = "";
        string movil = "";

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("sas", "DemoService started");

            StartServiceInForeground();


            //if (timer == null)
            //{
            //    timer = new System.Timers.Timer();
            //    timer.Interval = 180000;
            //    timer.Elapsed += Timer_Elapsed;
            //}
            //timer.Start();
            //DoWork ();

            return StartCommandResult.Sticky;
        }

        void StartServiceInForeground()
        {
            var ongoing = new Notification(Resource.Drawable.Icon, "Sas en Segundo Plano");
            // newActivity.PutExtra("ServiciosDet", servicio.ID);
            var newActivity = new Intent(this, typeof(Servicios));
            Bundle valuesForActivity = new Bundle();
            valuesForActivity.PutInt("GPS", 1);
            newActivity.PutExtras(valuesForActivity);
            var pendingIntent = PendingIntent.GetActivity(this, 0, newActivity, 0, valuesForActivity);
           ongoing.SetLatestEventInfo(this, "sas", "Sas se está ejecutando en segundo plano", pendingIntent);

            StartForeground((int)NotificationFlags.AutoCancel, ongoing);
            
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Debug("sas", "Service Started.");
            session = new UserSessionManager(this);
            IPCONN = session.getAccessConn();
            movil = session.getAccessIdmovil();

            if (timer == null)
            {
                timer = new System.Timers.Timer();
                timer.Interval = 180000;
                timer.Elapsed += Timer_Elapsed;
            }
            timer.Start();

        }

      private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {//DoWork();
           var t = new Thread(()  =>{
               BuscarServicios();
               });
           t.Start();
          
        }

        private async void BuscarServicios()
        {
            string result;
            try
            {
                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;
                client.BaseAddress = new System.Uri(IPCONN);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + session.getAccessToken());
                // string url = string.Format("/api/sas_ServiciosApi/{0}/{1}/{2}", user.codMovil.TrimEnd(), "001", "P");
                string url = string.Format("/api/sas_ServiciosApi/{0}/{1}/{2}", movil.TrimEnd(), "001", "P");
                var response = await client.GetAsync(url);
                result = response.Content.ReadAsStringAsync().Result;
                //Items = JsonConvert.DeserializeObject <List<Personas>> (result);
            }
            catch (Exception ex)
            {
              //  Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();
              return;
            }
            if (!(string.IsNullOrEmpty(result) || result == "null"))
            {
                SendNotification();
            }
        }

    
        public override void OnDestroy ()
		{
			base.OnDestroy ();
          
            if (!session.isLoggedIn())
            {
                timer.Stop();
                Log.Debug("SearchService", "SearchService stopped");
                StopForeground(true);
                StopSelf();
            }
              
		}

       

        void SendNotification ()
		{
            
            var nMgr = (NotificationManager)GetSystemService(NotificationService);
                    
            Intent newActivity = new Intent(this, typeof(Servicios));
        
            TaskStackBuilder stackBuilder = TaskStackBuilder.Create(this);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(Servicios)));
            stackBuilder.AddNextIntent(newActivity);

            // Create the PendingIntent with the back stack:            
            PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);

            Notification.Builder built = new Notification.Builder(this)
                .SetAutoCancel(true)
                .SetContentIntent(resultPendingIntent)
                .SetContentText("Existen Servicios pendientes")
                .SetSmallIcon(Resource.Drawable.notification);

        
            nMgr.Notify(0, built.Build());
           // timer.Stop();
            // nMgr.Notify(0, notification);
            return;

        }
   


        public override Android.OS.IBinder OnBind (Android.Content.Intent intent)
		{
			binder = new BuscarServiciosServiceBinder(this);
			return binder;
        }

		public string GetText ()
		{
			return "some text from the service";
		}


      
    }
   
    public class BuscarServiciosServiceBinder : Binder
	{
        BuscarServiciosService service;
    
		public BuscarServiciosServiceBinder(BuscarServiciosService service)
		{
			this.service = service;
		}

		public BuscarServiciosService GetDemoService ()
		{
			return service;
		}
    
    }
  
}