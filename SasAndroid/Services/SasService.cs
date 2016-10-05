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
using TaskyAndroid.Screens;
using System.Threading;
using Sas.Shared;

namespace SasAndroid.Services
{
    [Service]
    [IntentFilter(new String[] { "py.com.futura.SasService" })]
    public class SasService : Service
    {
        SasServiceBinder binder;
        IList<ServicioItem> servicios;
        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("SasService", "SasService iniciado");

            //Procesamos los parametros recibidos
            string tipo = intent.GetStringExtra("Tipo");
           

            if (tipo == null)
            {
                Log.Debug("parametro", "null");
                tipo = "BS";
            }
            else
            {
                Log.Debug("parametro", tipo);
                
            }

            StartServiceInForeground();

            DoWork(tipo);

            return StartCommandResult.NotSticky;
        }

        void StartServiceInForeground()
        {
            var ongoing = new Notification(Resource.Drawable.Icon, "Enviando datos al servidor");
            var pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(HomeScreen)), 0);
            ongoing.SetLatestEventInfo(this, "SasService", "SasService is running in the foreground", pendingIntent);

            StartForeground((int)NotificationFlags.ForegroundService, ongoing);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            Log.Debug("SasService", "SasService detenido");
        }

        void SendNotification(string mensaje)
        {
            var nMgr = (NotificationManager)GetSystemService(NotificationService);
            var notification = new Notification(Resource.Drawable.Icon, mensaje);
            var pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(HomeScreen)), 0);
            notification.SetLatestEventInfo(this, "sas Service Notification", mensaje, pendingIntent);
            
            nMgr.Notify(0, notification);
        }

        public void DoWork(String Tipo)
        {
           // Toast.MakeText(this, "The sas service has started", ToastLength.Long).Show();

            var t = new Thread(() => {

            switch (Tipo)
            {
                    case "ES":
                        EnviarServicios();
                        break;
                    case "BS":
                        BuscarServicios();
                        break;
                    case "AT":
                        ActualizarTablas();
                        break;

                    default:
                        break;
            }

            



                //Thread.Sleep(5000);

                Log.Debug("SasService", "Stopping foreground");
                StopForeground(true);

                StopSelf();
            }
            );

            t.Start();
        }

        public override Android.OS.IBinder OnBind(Android.Content.Intent intent)
        {
            binder = new SasServiceBinder(this);
            return binder;
        }

        void BuscarServicios()
        {

            Log.Debug("SasService", "Buscando servicios");
        }
        void EnviarServicios()
        {
            //Recuperar los datos a enviar al servidor
            //Verificar si hay conexion para Enviar al servidor
            //Enviar al servidor
            //Si el servidor responde correctamente, actualizar el estado enviado en la tabla correspondiente.


            servicios = ServicioItemManager.GetServiciosToSend();
            foreach (var item in servicios)
            {
                Log.Debug("SasService", String.Format("Enviando {0}", item.NroServicio));
                SendNotification(String.Format("Enviando {0}", item.NroServicio));
            }
        }


        void ActualizarTablas()
        {
            Log.Debug("SasService", "Actualizando Tablas");
        }

        public string GetText()
        {
            return "some text from the service";
        }
    }

    public class SasServiceBinder : Binder
    {
        SasService service;

        public SasServiceBinder(SasService service)
        {
            this.service = service;
        }

        public SasService GetSasService()
        {
            return service;
        }
    }
}