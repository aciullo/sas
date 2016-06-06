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
using sas.Clases;
using sas.Models;
using sas.Core;
using sas.Actividades;

namespace sas
{
    [Activity(Label = "Servicios", Theme = "@style/MyCustomTheme", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class Servicios  : Activity
    {
        private List<ServiciosModel> servicio;
        private DeviceUserModel user;
        IList<ServicioLocal> servicios = new List<ServicioLocal>();
        TextView txtTitulo;
        ListView lstServicios;
        ProgressBar mProgress;
        Button btnCerrarSesion;
        // private ArrayAdapter<ServiciosModel> ListAdapter;
        private System.Timers.Timer timer = new System.Timers.Timer();

        // Session Manager Class
        UserSessionManager session;
        string IPCONN = "";
        //bool isBound = false;
        //bool isConfigurationChange = false;
        // DemoServiceBinder binder;
        // DemoServiceConnection demoServiceConnection;

        protected override  void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ServicesLayout);
           // var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            //Toolbar will now take on default Action Bar characteristics
          //  SetActionBar(toolbar);

            // Session class instance
            session = new UserSessionManager(this);

            //Toast.MakeText(this, "User Login Status: " + session.isLoggedIn(), ToastLength.Long).Show();

            /**
        * Call this function whenever you want to check user login
        * This will redirect user to LoginActivity is he is not
        * logged in
        * */
            session.checkLogin();




            // Create your application here
            //strUser = Intent.Extras.GetString("user");
            //var deviceUser = JsonConvert.DeserializeObject<List<DeviceUserModel>>(strUser);
            //user = deviceUser;
            //  ActionBar.Title = "Ingreso al sistema";
            
            
            // get user data from session
          //  Dictionary<string, string> userLOG = session.getUserDetails();

            //  string nombre;
            //userLOG.TryGetValue(UserSessionManager.PREFERENCE_USER, out nombre);
            //string codmovil;
            //userLOG.TryGetValue(UserSessionManager.PREFERENCE_IDMOVIL, out codmovil);
            string nombre = session.getAccessKey();
            string codmovil = session.getAccessIdmovil();

            IPCONN = session.getAccessConn();

         
            user = new DeviceUserModel("","",codmovil.ToString(),nombre.ToString(),"");
                
          
            
            //user= new DeviceUserModel(user.usuario, user. pass, )

            mProgress = FindViewById<ProgressBar>(Resource.Id.mProgress);
            lstServicios = FindViewById<ListView>(Resource.Id.android_lstServicios);
            txtTitulo = FindViewById<TextView>(Resource.Id.txtTitulo);
            btnCerrarSesion = FindViewById<Button>(Resource.Id.btnCerrarSesion);
            if (user != null)
            {
                txtTitulo.Text = string.Format("Bienvenid@  {0}", user.nombres);
            }


            //mProgress.Indeterminate = true;
            //mProgress.Visibility = ViewStates.Visible;
            //// await LoadServicios();
            // ThreadPool.QueueUserWorkItem(async o => await LoadServicios());
            //mProgress.Visibility = ViewStates.Gone;

            lstServicios.ItemClick += LstServicios_ItemClick; ;
            
            //btnCerrarSesion.Click += BtnCerrarSesion_Click;
            

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
                //timer = new System.Timers.Timer();
                //timer.Interval = 150000;
                //timer.Elapsed += Timer_Elapsed;
                //timer.Start();
          
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.home, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {

         
            
            //Toast.MakeText(this, "Top ActionBar pressed: " + item.TitleFormatted, ToastLength.Short).Show();

            if (item.TitleFormatted.ToString() == "Sincronizar Datos")
            {
                //sincrionizar datos sas_datos

                 GetIndexDato("07");
                 GetIndexDato("06");
                //Toast.MakeText(this, "Sincronizacion completa", ToastLength.Long).Show();
               
            }

            if (item.TitleFormatted.ToString() == "Cerrar Sesión")
            {
                session.logoutUser();
                Finish();
               
            }


             return  base.OnOptionsItemSelected(item);





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
            //timer = new System.Timers.Timer();
            //// timer.Interval = 180000;
            //timer.Interval = 180000;
            //timer.Elapsed += Timer_Elapsed;
            //// }
            //timer.Start();

            //Timer_Elapsed(null, null);

            StartService(new Intent("com.xamarin.sas"));
        }
   
        private void LstServicios_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var t = servicios[e.Position];
            //Android.Widget.Toast.MakeText(this, t.nombrePaciente, Android.Widget.ToastLength.Short).Show();
            //Console.WriteLine("Clicked on " + t.nombrePaciente);
            var cantidad = ServicioManager.CantidadPendiente();
            
          
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

            if (t.codEstado== "009")
            {
                //AlertDialog.Builder builder = new AlertDialog.Builder(this);
                //builder.SetTitle("Alerta");
                //builder.SetMessage("Aca mostrar el traking del servicio.");
                //builder.SetCancelable(false);
                //builder.SetPositiveButton("OK", delegate { return; });
                //builder.Show();

                var newActivity = new Intent(this, typeof(RegistrarServicioLocal));
                Bundle valuesForActivity = new Bundle();
                valuesForActivity.PutInt("ServiciosDet", t.ID);
                newActivity.PutExtras(valuesForActivity);
                StartActivity(newActivity);

                return;
            }

            if (t.codEstado != "001" )
            {
                var newActivity = new Intent(this, typeof(RegistrarServicio));

                Bundle valuesForActivity = new Bundle();
                valuesForActivity.PutInt("ServiciosDet", t.ID);
                newActivity.PutExtras(valuesForActivity);

                //newActivity.PutExtra("ServiciosDet", t.ID);
                StartActivity(newActivity);
               
            }
            else
            {
             var newActivity = new Intent(this, typeof(ServiciosDetalle));
                // newActivity.PutExtra("ServiciosDet", t.ID);
                Bundle valuesForActivity = new Bundle();
                valuesForActivity.PutInt("ServiciosDet", t.ID);
                newActivity.PutExtras(valuesForActivity);
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

                // client.BaseAddress = new Uri("http://181.120.121.221:88");
                client.BaseAddress = new Uri(IPCONN);

                string movil = user.codMovil;
                movil = movil.TrimEnd();
                // string url = string.Format("/api/sas_ServiciosApi/{0}/{1}/{2}", user.codMovil.TrimEnd(), "001", "P");
                string url = string.Format("/api/sas_ServiciosApi/00?idmovil={0}", movil);

                var response = await client.GetAsync(url);
                result = response.Content.ReadAsStringAsync().Result;
                //Items = JsonConvert.DeserializeObject <List<Personas>> (result);
                if (!(response.IsSuccessStatusCode))
                {
                    // return;
                }

            }
            catch (Exception ex)
            {

                Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();

                return;
            }
            finally
            {

                servicios = ServicioManager.GetTasks();

                lstServicios.ChoiceMode = ChoiceMode.Single;

                lstServicios.Adapter = new ServicesAdapter(this, servicios);
            }

            try
            {
                if (string.IsNullOrEmpty(result) || result == "null")
                {
                    // servicio = new List<ServiciosModel>();
                    //servicio = new List<ServiciosModel>();
                    // servicio.Clear();
                    // return;
                }
                else
                {
                    servicio = JsonConvert.DeserializeObject<List<ServiciosModel>>(result);
                }

                //  var servlocal = new ServiciosLocalModel(t.id_Solicitud, t.NumeroSolicitud, t.fecha_Llamado, t.hora_Llamado,
                //t.nombrePaciente, t.Tel, t.edadPaciente, t.nombrePaciente, t.direccionReferecia, t.direccionReferecia2, t.numeroCasa, t.referencia,
                //t.Motivo, t.nroSalida, t.codMovil, t.codChofer, t.Acompañante, t.observacion, t.Estado, t.codEstado, t.HoraEstado,
                //t.codMotivo1, t.codMotivo2, t.codMotivo3, t.OtroMotivo, t.codTipo, t.codInstitucion, t.codDesenlace, t.producto);
                //ServiciosLocalModel servlocal;
                //servlocal = servicio[0].Class ;
                //var datos = new DAServicioCab();

                ServicioLocal sl = new ServicioLocal();
                foreach (ServiciosModel t in servicio)
                {
                    if (!ServicioManager.CheckIsDataAlreadyInDBorNot("[ServicioCab]", "[id_solicitud]", t.id_Solicitud.ToString()))
                    {
                        sl.id_Solicitud = t.id_Solicitud;
                        sl.NumeroSolicitud = t.NumeroSolicitud;
                        sl.fecha_Llamado = t.fecha_Llamado;
                        sl.hora_Llamado = t.hora_Llamado;
                        sl.nombrePaciente = t.nombrePaciente;
                        sl.Tel = t.Tel;
                        sl.edadPaciente = t.edadPaciente;
                        sl.nombreSolicitante = t.nombreSolicitante;
                        sl.direccionReferecia = t.direccionReferecia;
                        sl.direccionReferecia2 = t.direccionReferecia2;
                        sl.numeroCasa = t.numeroCasa;
                        sl.referencia = t.referencia;
                        sl.Motivo = t.Motivo;
                        sl.nroSalida = t.nroSalida;
                        sl.codMovil = t.codMovil;
                        sl.codChofer = t.codChofer;
                        sl.Acompañante = t.Acompañante;
                        sl.observacion = t.observacion;
                        sl.Estado = t.Estado;
                        sl.codEstado = t.codEstado;
                        sl.HoraEstado = t.HoraEstado;
                        sl.codMotivo1 = t.codMotivo1;
                        sl.codMotivo2 = t.codMotivo2;
                        sl.codMotivo3 = t.codMotivo3;
                        sl.OtroMotivo = t.OtroMotivo;
                        sl.codTipo = t.codTipo;
                        sl.codInstitucion = t.codInstitucion;
                        sl.codDesenlace = t.codDesenlace;
                        sl.producto = t.producto;
                        ServicioManager.SaveTask(sl);
                    }
                    //servlocal = new ServiciosLocalModel(t.id_Solicitud, t.NumeroSolicitud, t.fecha_Llamado, t.hora_Llamado,
                    //                                    t.nombrePaciente, t.Tel, t.edadPaciente, t.nombrePaciente,
                    //                                    t.direccionReferecia, t.direccionReferecia2, t.numeroCasa, t.referencia,
                    //                                    t.Motivo, t.nroSalida, t.codMovil, t.codChofer, t.Acompañante,
                    //                                    t.observacion, t.Estado, t.codEstado, t.HoraEstado,
                    //                                    t.codMotivo1, t.codMotivo2, t.codMotivo3, t.OtroMotivo, t.codTipo,
                    //                                    t.codInstitucion, t.codDesenlace, t.producto);
                    //using (datos)
                    //{
                    //    datos.InsertServicio(servlocal);
                    //}
                }
                servicios = ServicioManager.GetTasks();
                //waitActivityIndicator.IsRunning = false;
                //   lstServicios. = servicio;
                //ListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, 0, items);
                //lstServicios.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1,  items);
                lstServicios.ChoiceMode = ChoiceMode.Single;
                lstServicios.Adapter = new ServicesAdapter(this, servicios);
                //  RunOnUiThread(() => lstServicios.Adapter = new ServicesAdapter(this, servicio));
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                return;
            }
        }

        async void GetIndexDato(string codtabla)
        {
            mProgress.Visibility = ViewStates.Visible;
            mProgress.Indeterminate = true;
            string result;
            try
            {
                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;
                client.BaseAddress = new Uri(IPCONN);
                string url = string.Format("/api/SasDatosApi?idtabla={0}", codtabla);
                var response = await client.GetAsync(url);
                result = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();
                return;
            }


            var din = JsonConvert.DeserializeObject<List<SasDatosItem>>(result);
            SasDatosItem sl = new SasDatosItem();
            foreach (SasDatosItem t in din)
            {
                if (!SasDatosManager.CheckIsDataAlreadyInDBorNot("[sasDatos]", "[codigo] ='" + t.codigo + "' and [idtabla] ='" + t.idtabla +  "'"))
                {
                    sl.codigo = t.codigo;
                    sl.descripcion = t.descripcion;
                    sl.idtabla = t.idtabla;
                    SasDatosManager.SaveTask(sl);
                }
                mProgress.Visibility = ViewStates.Gone;
            }
        }



        protected override void OnDestroy()
        {
            base.OnDestroy();
            //timer.Stop();

            //if (!isConfigurationChange)
            //{
            //    if (isBound)
            //    {
            //        UnbindService(demoServiceConnection);
            //        isBound = false;
            //    }
            //}
        }

        #region "codigo comentado "


        //private void BtnCerrarSesion_Click(object sender, EventArgs e)
        //{
        //    session.logoutUser();
        //}

        //private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    //DoWork();
        //    var t = new Thread(() => {
        //        BuscarServicios();
        //    });

        //    t.Start();
        //}

        //private async void BuscarServicios()
        //{
        //    string result;


        //    try
        //    {

        //        HttpClient client = new HttpClient();
        //        client.MaxResponseContentBufferSize = 256000;

        //        // client.BaseAddress = new Uri("http://181.120.121.221:88");
        //        client.BaseAddress = new Uri(IPCONN);

        //        // string url = string.Format("/api/sas_ServiciosApi/{0}/{1}/{2}", user.codMovil.TrimEnd(), "001", "P");
        //        string url = string.Format("/api/sas_ServiciosApi/{0}/{1}/{2}", user.codMovil.TrimEnd(), "001", "P");

        //        var response = await client.GetAsync(url);
        //        result = response.Content.ReadAsStringAsync().Result;
        //        //Items = JsonConvert.DeserializeObject <List<Personas>> (result);

        //    }
        //    catch (Exception ex)
        //    {

        //        Toast.MakeText(this, ex.Message, ToastLength.Long).Show();

        //        return;
        //    }

        //    if (string.IsNullOrEmpty(result) || result == "null")
        //    {


        //        return;


        //    }

        //    SendNotification();
        //    return;
        //}
        //void SendNotification()
        //{
        //    var nMgr = (NotificationManager)GetSystemService(NotificationService);
        //   // var notification = new Notification(Resource.Drawable.notification, "Existen Servicios pendientes");
        //    //Bundle valuesForActivity = new Bundle();
        //    //valuesForActivity.PutParcelable("user", user);
        //   // valuesForActivity.PutParcelable("userMobile", user);
        //    Intent newActivity = new Intent(this, typeof(Servicios));
        //    //newActivity.PutExtras(valuesForActivity);

        //    TaskStackBuilder stackBuilder = TaskStackBuilder.Create(this);
        //    stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(Servicios)));
        //    stackBuilder.AddNextIntent(newActivity);

        //    // Create the PendingIntent with the back stack:            
        //    PendingIntent resultPendingIntent =
        //        stackBuilder.GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);

        //    //var newActivity = new Intent(this, typeof(Servicios));
        //    //newActivity.PutExtra("user", user);
        //    //var pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(Servicios)), 0);
        //    // var pendingIntent = PendingIntent.GetActivity(this, 0, newActivity, PendingIntentFlags.UpdateCurrent);

        //    Notification.Builder built = new Notification.Builder(this)
        //        .SetAutoCancel(true)
        //        .SetContentIntent(resultPendingIntent)
        //        .SetContentText("Existen Servicios pendientes")
        //        .SetSmallIcon(Resource.Drawable.notification);

        //   // notification.SetLatestEventInfo(this, " Notificación ", "Existen Servicios pendientes", pendingIntent);
        //    // notification.ContentIntent = pendingIntent;
        //    nMgr.Notify(0, built.Build());
        //    timer.Stop();
        //    // nMgr.Notify(0, notification);
        //    return;
        //}

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
        #endregion
    }
}