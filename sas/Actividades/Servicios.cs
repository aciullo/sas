using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;

namespace sas
{
    [Activity(Label = "Servicios", Theme = "@style/MyCustomTheme")]
    public class Servicios  : Activity
    {
        private List<ServiciosModel> servicio;
        private List<DeviceUserModel> user;
        string strUser = "";
        TextView txtTitulo;
        ListView lstServicios;
        ProgressBar mProgress;

        // private ArrayAdapter<ServiciosModel> ListAdapter;
        private System.Timers.Timer timer = new System.Timers.Timer();
       
        //bool isBound = false;
        //bool isConfigurationChange = false;
       // DemoServiceBinder binder;
       // DemoServiceConnection demoServiceConnection;

        protected override  void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
             strUser = Intent.Extras.GetString("user");
             var deviceUser = JsonConvert.DeserializeObject<List<DeviceUserModel>>(strUser);
             user = deviceUser;
          //  ActionBar.Title = "Ingreso al sistema";

           
                
          
            SetContentView(Resource.Layout.ServicesLayout);
            //user= new DeviceUserModel(user.usuario, user. pass, )

            mProgress = FindViewById<ProgressBar>(Resource.Id.mProgress);
            lstServicios = FindViewById<ListView>(Resource.Id.android_lstServicios);
            txtTitulo = FindViewById<TextView>(Resource.Id.txtTitulo);

            if (user != null)
            {
                txtTitulo.Text = string.Format("Bienvenid@  {0}", user[0].nombres + " " + user[0].apellidos);
            }


            //mProgress.Indeterminate = true;
            //mProgress.Visibility = ViewStates.Visible;
            //// await LoadServicios();
            // ThreadPool.QueueUserWorkItem(async o => await LoadServicios());
            //mProgress.Visibility = ViewStates.Gone;

            lstServicios.ItemClick += LstServicios_ItemClick; ;

            // StartService(new Intent(this, typeof(DemoService)));

            // StartService(new Intent("com.xamarin.sas"));
            // restore from connection there was a configuration change, such as a device rotation



            //demoServiceConnection = LastNonConfigurationInstance as DemoServiceConnection;

            //if (demoServiceConnection != null)
            //    binder = demoServiceConnection.Binder;

            //  if (timer == null)
            //  {
            //     timer = new System.Timers.Timer();
            //     timer.Interval = 15000;
            //     timer.Elapsed += Timer_Elapsed;
            //// }
            //timer.Stop();
            // timer.Dispose();
            // Timer_Elapsed(null, null);
                timer = new System.Timers.Timer();
                timer.Interval = 150000;
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
          
        }


        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //DoWork();
            var t = new Thread(() => {
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

                client.BaseAddress = new Uri("http://181.120.121.221:88");


                // string url = string.Format("/api/sas_ServiciosApi/{0}/{1}/{2}", user.codMovil.TrimEnd(), "001", "P");
                string url = string.Format("/api/sas_ServiciosApi/{0}/{1}/{2}", user[0].codMovil.TrimEnd(), "001", "P");

                var response = await client.GetAsync(url);
                result = response.Content.ReadAsStringAsync().Result;
                //Items = JsonConvert.DeserializeObject <List<Personas>> (result);

            }
            catch (Exception ex)
            {

                Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();

                return;
            }

            if (string.IsNullOrEmpty(result) || result == "null")
            {


                return;
              

            }

            SendNotification();
            return;
        }
        void SendNotification()
        {
            var nMgr = (NotificationManager)GetSystemService(NotificationService);
           // var notification = new Notification(Resource.Drawable.notification, "Existen Servicios pendientes");
            Bundle valuesForActivity = new Bundle();
            valuesForActivity.PutString("user", strUser);
           // valuesForActivity.PutParcelable("userMobile", user);
            Intent newActivity = new Intent(this, typeof(Servicios));
            newActivity.PutExtras(valuesForActivity);

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
        protected override async void OnStart()
        {
            base.OnStart();
            mProgress.Indeterminate = true;
            mProgress.Visibility = ViewStates.Visible;
             await LoadServicios();
            //var demoServiceIntent = new Intent("com.xamarin.sas");
            //demoServiceConnection = new DemoServiceConnection(this);
            //ApplicationContext.BindService(demoServiceIntent, demoServiceConnection, Bind.AutoCreate);
            mProgress.Visibility = ViewStates.Gone;
        }
        protected override void OnUserLeaveHint()
        {
            base.OnUserLeaveHint();
            //  StartService(new Intent("com.xamarin.sas"));
            timer = new System.Timers.Timer();
            // timer.Interval = 180000;
            timer.Interval = 150000;
            timer.Elapsed += Timer_Elapsed;
            // }
            timer.Start();

            //Timer_Elapsed(null, null);
        }
   
        private void LstServicios_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var t = servicio[e.Position];
            //Android.Widget.Toast.MakeText(this, t.nombrePaciente, Android.Widget.ToastLength.Short).Show();
            //Console.WriteLine("Clicked on " + t.nombrePaciente);
            var serPendientes = servicio.FindAll(x => x.codEstado != "001" && x.codEstado != "009");
            
            int cantidad = serPendientes.Count;
            if (cantidad >= 1 && t.codEstado=="001")
            {

                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Alerta");
                builder.SetMessage("Finalize los servicios antes de iniciar uno nuevo.");
                builder.SetCancelable(false);
                builder.SetPositiveButton("OK", delegate { return; });
                builder.Show();
                return;
            }

            if (t.codEstado != "001" )
            {
                var newActivity = new Intent(this, typeof(RegistrarServicio));
                newActivity.PutExtra("ServiciosDet", t);
                StartActivity(newActivity);
            }
            else
            {
             var newActivity = new Intent(this, typeof(ServiciosDetalle));
                 newActivity.PutExtra("ServiciosDet", t);
                 StartActivity(newActivity);

            }
           
          

        }



        private async Task LoadServicios()
        {
            string result;
            Toast.MakeText(this, "Buscando servicios...aguarde por favor...", ToastLength.Long).Show();

            try
            {

                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;

                client.BaseAddress = new Uri("http://181.120.121.221:88");


                // string url = string.Format("/api/sas_ServiciosApi/{0}/{1}/{2}", user.codMovil.TrimEnd(), "001", "P");
                string url = string.Format("/api/sas_ServiciosApi/00?idmovil={0}", user[0].codMovil.TrimEnd());

                var response = await client.GetAsync(url);
                result = response.Content.ReadAsStringAsync().Result;
                //Items = JsonConvert.DeserializeObject <List<Personas>> (result);
                if (!(response.IsSuccessStatusCode))
                {
                    return;
                }

            }
            catch (Exception ex)
            {
              
                Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();
               
                return;
            }

            try
            {
               

                if (string.IsNullOrEmpty(result) || result == "null")
                {
                    // servicio = new List<ServiciosModel>();
                    servicio = new List<ServiciosModel>();

                    servicio.Clear();
                   // return;
                  
                }
                else
                {
                    servicio = JsonConvert.DeserializeObject<List<ServiciosModel>>(result);


                }
                    //waitActivityIndicator.IsRunning = false;
                 
                
                //   lstServicios. = servicio;

               

                //ListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, 0, items);

                //lstServicios.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1,  items);

                lstServicios.ChoiceMode = ChoiceMode.Single;

                lstServicios.Adapter = new ServicesAdapter(this, servicio);
              //  RunOnUiThread(() => lstServicios.Adapter = new ServicesAdapter(this, servicio));

            }
            catch (Exception ex)
            {
               
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
               
                return;
            }

       
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            timer.Stop();

            //if (!isConfigurationChange)
            //{
            //    if (isBound)
            //    {
            //        UnbindService(demoServiceConnection);
            //        isBound = false;
            //    }
            //}
        }

        // return the service connection if there is a configuration change
        //public override Java.Lang.Object OnRetainNonConfigurationInstance()
        //{
        //    base.OnRetainNonConfigurationInstance();

        //    isConfigurationChange = true;

        //    return demoServiceConnection;
        //}

        //class DemoServiceConnection : Java.Lang.Object, IServiceConnection
        //{
        //    Servicios activity;
        //    DemoServiceBinder binder;

        //    public DemoServiceBinder Binder
        //    {
        //        get
        //        {
        //            return binder;
        //        }
        //    }

        //    public DemoServiceConnection(Servicios activity)
        //    {
        //        this.activity = activity;
        //    }

        //    public void OnServiceConnected(ComponentName name, IBinder service)
        //    {
        //        var demoServiceBinder = service as DemoServiceBinder;
        //        if (demoServiceBinder != null)
        //        {
        //            var binder = (DemoServiceBinder)service;
        //            activity.binder = binder;
        //            activity.isBound = true;

        //            // keep instance for preservation across configuration changes
        //            this.binder = (DemoServiceBinder)service;
        //        }
        //    }

        //    public void OnServiceDisconnected(ComponentName name)
        //    {
        //        activity.isBound = false;
        //    }
        //}

    }
}