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

namespace sas
{
    [Service]
    [IntentFilter(new String[] { "com.xamarin.sas" })]
    public class DemoService : Service
    {
        DemoServiceBinder binder;
        IList<ServicioItem> servicios;
        ServicioItem servicioDetalle;
        private System.Timers.Timer timer= new System.Timers.Timer();
        private System.Timers.Timer timerSinc = new System.Timers.Timer();
        private int counter = 0, incrementby = 1;
        UserSessionManager session;
        string IPCONN = "";


        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("sas", "DemoService started");

            StartServiceInForeground ();

            //DoWork ();

            return StartCommandResult.Sticky;
        }

        void StartServiceInForeground()
        {
            var ongoing = new Notification(Resource.Drawable.Icon, "Sas en Segundo Plano");
            var pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(Servicios)), 0);
            ongoing.SetLatestEventInfo(this, "sas", "Sas se está ejecutando en segundo plano", pendingIntent);

            StartForeground((int)NotificationFlags.AutoCancel, ongoing);
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Debug("sas", "Service Started.");

            session = new UserSessionManager(this);

            IPCONN = session.getAccessConn();


            timer = new System.Timers.Timer();
            timer.Interval = 180000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            timerSinc = new System.Timers.Timer();
            timerSinc.Interval = 185000;
            timerSinc.Elapsed += TimerSinc_Elapsed;
            timerSinc.Start();


        }

        private void TimerSinc_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (NetworkConnected(this))
            {
                var t = new Thread(() => { SincronizarEstados(); });
                t.Start();
            }
            timerSinc.Stop();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {//DoWork();
           var t = new Thread(()  =>{ BuscarServicios(); });
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
                // string url = string.Format("/api/sas_ServiciosApi/{0}/{1}/{2}", user.codMovil.TrimEnd(), "001", "P");
                string url = string.Format("/api/sas_ServiciosApi/{0}/{1}/{2}", "10", "001", "P");
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

        private async void SincronizarEstados()
        {
            servicios = ServicioItemManager.GetServiciosToSend();
            foreach (var item in servicios)
            {

                string result;
                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;
                System.Net.Http.HttpResponseMessage response;
                client.BaseAddress = new System.Uri(IPCONN);
                string url;

                try
                {

                    url = string.Format("/api/UpdServiciosApi?idsolicitud={0}&codestado={1}&hora={2}", item.id_Solicitud, item.codEstado, item.HoraEstado);
                    response = await client.GetAsync(url);
                    result = response.Content.ReadAsStringAsync().Result;

                    if (result.Contains("Error"))
                    {
                      //  Toast.MakeText(this, "Error", ToastLength.Long).Show();



                        servicioDetalle = new ServicioItem();
                        servicioDetalle.ID = item.ID;
                        servicioDetalle.Enviado = false;
                        ServicioItemManager.SaveTask(servicioDetalle);
                      

                    }
                    else
                    {
                        servicioDetalle = new ServicioItem();
                        servicioDetalle.ID = item.ID;
                        servicioDetalle.Enviado = true;
                        ServicioItemManager.SaveTask(servicioDetalle);
                        Log.Debug("SasService", String.Format("Enviando {0}", item.NumeroSolicitud));
                        SendNotification(String.Format("Enviando {0}", item.NumeroSolicitud));
                    }


                    if ((!string.IsNullOrEmpty(item.codDesenlace ) ) && (string.IsNullOrEmpty(item.codInstitucion) || item.codInstitucion=="Null"))
                    {
                        item.codInstitucion = "Null";
                        //url = string.Format("/api/ABMServiciosApi?idsolicitud={0}&nrosolicitud={1}&destino={2}", item.id_Solicitud, item.NumeroSolicitud, item.codInstitucion);
                        url = (string.Format("/api/ABMServiciosApi?idsolicitud={0}&nrosolicitud={1}&destino={2}&desenlace={3}", item.id_Solicitud, item.NumeroSolicitud, item.codInstitucion, item.codDesenlace));
                        response = await client.GetAsync(url);
                        result = response.Content.ReadAsStringAsync().Result;

                        if (result.Contains("Error"))
                        {
                            // Toast.MakeText(this, "Error", ToastLength.Long).Show();



                            servicioDetalle = new ServicioItem();
                            servicioDetalle.ID = item.ID;
                            servicioDetalle.Enviado = false;
                            ServicioItemManager.SaveTask(servicioDetalle);
                            return;

                        }


                    }
                    else
                    {
                        if ((string.IsNullOrEmpty(item.codDesenlace) || item.codDesenlace=="Null") && (!string.IsNullOrEmpty(item.codInstitucion)))
                        {
                            item.codDesenlace = "Null";
                            url = string.Format("/api/ABMServiciosApi?idsolicitud={0}&nrosolicitud={1}&destino={2}", item.id_Solicitud, item.NumeroSolicitud, item.codInstitucion);
                            response = await client.GetAsync(url);

                            if (result.Contains("Error"))
                            {
                                // Toast.MakeText(this, "Error", ToastLength.Long).Show();



                                servicioDetalle = new ServicioItem();
                                servicioDetalle.ID = item.ID;
                                servicioDetalle.Enviado = false;
                                ServicioItemManager.SaveTask(servicioDetalle);
                                return;

                            }
                        }
                    }

                 
                       

                   
                }
                catch (Exception ex)
                {

                    servicioDetalle = new ServicioItem();
                    servicioDetalle.ID = item.ID;
                    servicioDetalle.Enviado = false;
                    ServicioItemManager.SaveTask(servicioDetalle);
                    return;
                }


               
            }

            //Thread.Sleep(5000);
            timerSinc.Start();
            Log.Debug("SasService", "Stopping foreground");
            StopForeground(true);
            StopSelf();
        }

        void SendNotification(string mensaje)
        {
            var nMgr = (NotificationManager)GetSystemService(NotificationService);
            //var notification = new Notification(Resource.Drawable.Icon, mensaje);
            //    var pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(Servicios)), 0);
            //  notification.SetLatestEventInfo(this, "sas Service Notification", "Message from sas service", pendingIntent);
            Notification.Builder built = new Notification.Builder(this)
                .SetAutoCancel(true)
                .SetContentText(mensaje)
                .SetSmallIcon(Resource.Drawable.Icon);

            nMgr.Notify(0, built.Build());
        }

        public override void OnDestroy ()
		{
			base.OnDestroy ();
          
            if (!session.isLoggedIn())
            {
                timer.Stop();
                timerSinc.Stop();
                Log.Debug("sas", "DemoService stopped");
                StopForeground(true);
                StopSelf();
            }
              
		}

        public static bool NetworkConnected(Context context)
        {
            ConnectivityManager connectivity = ((ConnectivityManager)context.GetSystemService(Context.ConnectivityService));
            if (connectivity != null)
            {

                if (connectivity != null)
                {
                    NetworkInfo[] info = connectivity.GetAllNetworkInfo();

                    if (info != null)
                    {
                        for (int i = 0; i < info.Length; i++)
                        {
                            if (info[i].GetState() == NetworkInfo.State.Connected)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        void SendNotification ()
		{
            //original
            //var nMgr = (NotificationManager)GetSystemService (NotificationService);
            //var notification = new Notification (Resource.Drawable.Icon, "Existen Servicios pendientes");
            //var pendingIntent = PendingIntent.GetActivity (this, 0, new Intent (this, typeof(Servicios)),0);
            //         notification.SetLatestEventInfo (this, " Notificación ", "Existen Servicios pendientes", pendingIntent);
            //        // notification.ContentIntent = pendingIntent;
            //         nMgr.Notify (0, notification);

            var nMgr = (NotificationManager)GetSystemService(NotificationService);
            
            //Bundle valuesForActivity = new Bundle();
            //valuesForActivity.PutString("user", strUser);
            
            Intent newActivity = new Intent(this, typeof(Servicios));
            //newActivity.PutExtras(valuesForActivity);

            TaskStackBuilder stackBuilder = TaskStackBuilder.Create(this);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(Servicios)));
            stackBuilder.AddNextIntent(newActivity);

            // Create the PendingIntent with the back stack:            
            PendingIntent resultPendingIntent =
                stackBuilder.GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);

            //var newActivity = new Intent(this, typeof(Servicios));
            //newActivity.PutExtra("user", user);
            //var pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(Servicios)), 0);
            // var pendingIntent = PendingIntent.GetActivity(this, 0, newActivity, PendingIntentFlags.UpdateCurrent);

            Notification.Builder built = new Notification.Builder(this)
                .SetAutoCancel(true)
                .SetContentIntent(resultPendingIntent)
                .SetContentText("Existen Servicios pendientes")
                .SetSmallIcon(Resource.Drawable.notification);

            // notification.SetLatestEventInfo(this, " Notificación ", "Existen Servicios pendientes", pendingIntent);
            // notification.ContentIntent = pendingIntent;
            nMgr.Notify(0, built.Build());
            timer.Stop();
            // nMgr.Notify(0, notification);
            return;

        }

		//public void DoWork ()
		//{
		//	//Toast.MakeText (this, "The demo service has started", ToastLength.Long).Show ();

		//	var t = new Thread (async () =>
  //          {

  //              string result;


  //              try
  //              {

  //                  HttpClient client = new HttpClient();
  //                  client.MaxResponseContentBufferSize = 256000;

  //                  client.BaseAddress = new System.Uri("http://181.120.121.221:88");


  //                  // string url = string.Format("/api/sas_ServiciosApi/{0}/{1}/{2}", user.codMovil.TrimEnd(), "001", "P");
  //                  string url = string.Format("/api/sas_ServiciosApi/{0}/{1}/{2}", "10", "001", "P");

  //                  var response = await client.GetAsync(url);
  //                  result = response.Content.ReadAsStringAsync().Result;
  //                  //Items = JsonConvert.DeserializeObject <List<Personas>> (result);

  //              }
  //              catch (Exception ex)
  //              {

  //                  Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();

  //                  return;
  //              }

  //              if (!(string.IsNullOrEmpty(result) || result == "null"))
  //              {

  //                  SendNotification();

                   
  //              }

               

  //              Thread.Sleep(500);

  //             //Log.Debug("sas", "Stopping foreground");
  //            // StopForeground(true);

  //            // StopSelf();
  //          }
  //          );

		//	t.Start ();
		//}
      


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