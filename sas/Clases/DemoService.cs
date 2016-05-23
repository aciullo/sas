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

namespace sas
{
    [Service]
    [IntentFilter(new String[] { "com.xamarin.sas" })]
    public class DemoService : Service
    {
        DemoServiceBinder binder;

        private System.Timers.Timer timer= new System.Timers.Timer();
        private int counter = 0, incrementby = 1;

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("sas", "DemoService started");

            //StartServiceInForeground ();

            //DoWork ();

            return StartCommandResult.Sticky;
        }

        void StartServiceInForeground()
        {
            var ongoing = new Notification(Resource.Drawable.Icon, "DemoService in foreground");
            var pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(Servicios)), 0);
            ongoing.SetLatestEventInfo(this, "sas", "DemoService is running in the foreground", pendingIntent);

            StartForeground((int)NotificationFlags.ForegroundService, ongoing);
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Debug("sas", "Service Started.");
            if (timer == null)
            {
                timer = new System.Timers.Timer();
                timer.Interval = 15000;
                timer.Elapsed += Timer_Elapsed;
            }
            timer.Start();

            Timer_Elapsed(null, null);


        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
           //DoWork();
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

                client.BaseAddress = new Uri("http://192.168.0.13");


                // string url = string.Format("/sas_Futura/api/sas_ServiciosApi/{0}/{1}/{2}", user.codMovil.TrimEnd(), "001", "P");
                string url = string.Format("/sas_Futura/api/sas_ServiciosApi/{0}/{1}/{2}", "10", "001", "P");

                var response = await client.GetAsync(url);
                result = response.Content.ReadAsStringAsync().Result;
                //Items = JsonConvert.DeserializeObject <List<Personas>> (result);

            }
            catch (Exception ex)
            {

                Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();

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
            timer.Stop();
            
			Log.Debug ("sas", "DemoService stopped");       
		}

		void SendNotification ()
		{
			var nMgr = (NotificationManager)GetSystemService (NotificationService);
			var notification = new Notification (Resource.Drawable.Icon, "Existen Servicios pendientes");
			var pendingIntent = PendingIntent.GetActivity (this, 0, new Intent (this, typeof(Servicios)),0);
            notification.SetLatestEventInfo (this, " Notificación ", "Existen Servicios pendientes", pendingIntent);
           // notification.ContentIntent = pendingIntent;
            nMgr.Notify (0, notification);
           
		}

		public void DoWork ()
		{
			//Toast.MakeText (this, "The demo service has started", ToastLength.Long).Show ();

			var t = new Thread (async () =>
            {

                string result;


                try
                {

                    HttpClient client = new HttpClient();
                    client.MaxResponseContentBufferSize = 256000;

                    client.BaseAddress = new Uri("http://192.168.0.13");


                    // string url = string.Format("/sas_Futura/api/sas_ServiciosApi/{0}/{1}/{2}", user.codMovil.TrimEnd(), "001", "P");
                    string url = string.Format("/sas_Futura/api/sas_ServiciosApi/{0}/{1}/{2}", "10", "001", "P");

                    var response = await client.GetAsync(url);
                    result = response.Content.ReadAsStringAsync().Result;
                    //Items = JsonConvert.DeserializeObject <List<Personas>> (result);

                }
                catch (Exception ex)
                {

                    Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();

                    return;
                }

                if (!(string.IsNullOrEmpty(result) || result == "null"))
                {

                    SendNotification();

                   
                }

               

                Thread.Sleep(500);

               //Log.Debug("sas", "Stopping foreground");
              // StopForeground(true);

              // StopSelf();
            }
            );

			t.Start ();
		}
      


        public override Android.OS.IBinder OnBind (Android.Content.Intent intent)
		{
			binder = new DemoServiceBinder (this);
			return binder;
        }

		public string GetText ()
		{
			return "some text from the service";
		}


      
    }
   
    public class DemoServiceBinder : Binder
	{
		DemoService service;
    
		public DemoServiceBinder (DemoService service)
		{
			this.service = service;
		}

		public DemoService GetDemoService ()
		{
			return service;
		}
    
    }
  
}