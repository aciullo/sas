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
using Android.Graphics;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using sas.Actividades;
using sas.Clases;
using sas.Core;
using Android.Locations;
using Android.Util;
using Android.Gms.Common;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace sas
{
    [Activity(Label = "Servicios", Theme = "@style/MyCustomTheme", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class Servicios  : Activity, ILocationListener
    {
        private List<ServiciosModel> servicio;
        private DeviceUserModel user;
        IList<ServicioLocalItem> servicios = new List<ServicioLocalItem>();
        TextView txtTitulo;
        ListView lstServicios;
        ProgressBar mProgress;
        Button btnCerrarSesion;
        
        // private ArrayAdapter<ServiciosModel> ListAdapter;
        //private System.Timers.Timer timer = new System.Timers.Timer();

        // Session Manager Class
        UserSessionManager session;
        string IPCONN = "";
        //bool isBound = false;
        //bool isConfigurationChange = false;
        // DemoServiceBinder binder;
        // DemoServiceConnection demoServiceConnection;

        //GPS
        Location _currentLocation;
        LocationManager _locationManager;
        string _locationProvider;
        string _locationText;
        int verGPS;
        static readonly string TAG = "X:" + typeof(Servicios).Name;

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

         
            user = new DeviceUserModel("","",codmovil.ToString(),nombre.ToString(),"","");
                
          
            
            //user= new DeviceUserModel(user.usuario, user. pass, )

            mProgress = FindViewById<ProgressBar>(Resource.Id.mProgress);
            lstServicios = FindViewById<ListView>(Resource.Id.android_lstServicios);
            txtTitulo = FindViewById<TextView>(Resource.Id.txtTitulo);
            btnCerrarSesion = FindViewById<Button>(Resource.Id.btnCerrarSesion);

            if (user != null)
            {
                txtTitulo.Text = string.Format("Bienvenid@  {0}", user.nombres);
            }

            if (Intent.Extras == null)
            {
               
                verGPS = 1;
            }
           else
            {
                GetIndexDato("07");
                GetIndexDato("06");
                verGPS = Intent.Extras.GetInt("GPS");
            }
            
     
            lstServicios.ItemClick += LstServicios_ItemClick; ;

            if (IsPlayServicesAvailable())
            {
                var intent = new Intent(this, typeof(RegistrationIntentService));
                StartService(intent);
            }



        }

        public bool IsPlayServicesAvailable()
        {
            string msgText = "";
            //AlertDialog.Builder builder = new AlertDialog.Builder(this);
            //builder.SetTitle("Aviso");

            //builder.SetCancelable(true);
            //builder.SetPositiveButton("OK", delegate { return; });

            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    msgText = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                    //builder.SetMessage(msgText);
                    //builder.Show();
                }
                else
                {
                    msgText = "Sorry, this device is not supported";
                    //builder.SetMessage(msgText);
                    //builder.Show();
                   
                }
                return false;
            }
            else
            {
                msgText = "Google Play Services is available.";
                //builder.SetMessage(msgText);
                //builder.Show();
                return true;
            }
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
                mProgress.Visibility = ViewStates.Visible;
                mProgress.Indeterminate = true;
                //sincrionizar datos sas_datos
                GetIndexDato("07");
                GetIndexDato("06");
                //Toast.MakeText(this, "Sincronizacion completa", ToastLength.Long).Show();
            }
            if (item.TitleFormatted.ToString() == "Cerrar Sesión")
            {
                session.logoutUser();
               // StopService(new Intent("com.sas.searchpending"));
                StopService(new Intent("com.xamarin.sas"));
                Finish();
            }

            if (item.TitleFormatted.ToString() == "Enviar notificación")
            {
                MandarNotificacion();
            }

            if (item.TitleFormatted.ToString() == "Acerca De")
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Información");
                builder.SetMessage("Futura Software Versión 1.0 Copyright © 2016 - Virginio González");
                builder.SetPositiveButton("OK", delegate
                { return; });
                builder.SetCancelable(false);
              

                Dialog alertDialog = builder.Create();
                alertDialog.SetCanceledOnTouchOutside(false);
                alertDialog.Show();
            }
            return base.OnOptionsItemSelected(item);
        }
        private void MandarNotificacion()
        {
          
             string MESSAGE = "Nuevo Servicio ";
             string resultado = "";
             var jGcmData = new JObject();
             var jData = new JObject();

                jData.Add("message", MESSAGE);
                // jGcmData.Add("to", "/topics/global");
                jGcmData.Add("to", user.idRegistro);
                //jGcmData.Add("to", user.idRegistro + "/topics/global");

                jGcmData.Add("data", jData);

                var url = new Uri("https://gcm-http.googleapis.com/gcm/send");
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));

                        client.DefaultRequestHeaders.TryAddWithoutValidation(
                            "Authorization", "key=" + Constantes.API_KEY);

                        Task.WaitAll(client.PostAsync(url,
                            new StringContent(jGcmData.ToString(), Encoding.Default, "application/json"))
                                .ContinueWith(response =>
                                {
                                    resultado = response.Status.ToString();
                                    resultado = resultado + " " + response.Result.ToString();
                                    resultado = resultado + " " + "Message sent: check the client device notification tray.";
                                }));
                    }
                }
                catch (Exception e)
                {
                    resultado = ("Unable to send GCM message:");
                    resultado = resultado + " STRACKTRACE:  " + (e.StackTrace);
                }
            Log.Debug(TAG, "resultado " + resultado + ".");
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
        void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.NoRequirement,
                PowerRequirement = Power.NoRequirement


            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders (criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = string.Empty;
            }
            Log.Debug(TAG, "Using " + _locationProvider + ".");
        }

        protected override void OnPause()
        {
            base.OnPause();
            //_locationManager.RemoveUpdates(this);
            //Log.Debug(TAG, "No longer listening for location updates.");
        }
        protected override void OnResume()
        {
            base.OnResume();

             _locationManager = GetSystemService(Context.LocationService) as LocationManager;
            InitializeLocationManager();

            //if (_locationManager.IsProviderEnabled(LocationManager.GpsProvider))
            if ((!string.IsNullOrEmpty(_locationProvider)) )
            {
                _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
                Log.Debug(TAG, "Listening for location updates using " + _locationProvider + ".");

            }
            else
            {
                if (verGPS == 0)
                {
                    GetIndexDato("07");
                    GetIndexDato("06");
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetTitle("El servicio de localización no se encuentra activo");
                    builder.SetMessage("Por favor habilite el Servicio de Localización y GPS");
                    builder.SetPositiveButton("OK", delegate
                    {
                        // Show location settings when the user acknowledges the alert dialog
                        Intent intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                        StartActivity(intent);

                    });
                    builder.SetCancelable(true);
                    builder.SetNegativeButton("Cancelar", delegate { return; });

                    Dialog alertDialog = builder.Create();
                    alertDialog.SetCanceledOnTouchOutside(false);
                    alertDialog.Show();
                    verGPS = 1;

                }
            }
        }
        public void OnLocationChanged(Location location)
        {
            _currentLocation = location;
            if (_currentLocation == null)
            {
                _locationText = "Unable to determine your location. Try again in a short while.";
            }
            else
            {
                _locationText = string.Format("{0:f6},{1:f6}", _currentLocation.Latitude, _currentLocation.Longitude);
                // Address address = await ReverseGeocodeCurrentLocation();
                // DisplayAddress(address);

            }
        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Log.Debug(TAG, "{0}, {1}", provider, status);
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

            

        }

        private  void LstServicios_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var t = servicios[e.Position];
            //Android.Widget.Toast.MakeText(this, t.nombrePaciente, Android.Widget.ToastLength.Short).Show();
            //Console.WriteLine("Clicked on " + t.nombrePaciente);
            var cantidad = ServicioManager.CantidadPendiente();


            if (cantidad >= 1 && t.codEstado == "002")
            {

                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Alerta");
                builder.SetMessage("Finalize los servicios antes de iniciar uno nuevo.");
                builder.SetCancelable(false);
                builder.SetPositiveButton("OK", delegate { return; });
                builder.Show();
                return;
            }

            if (t.codEstado == "009")
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

            //if (t.codEstado == "008")
            //{
            //    //
            //    if (! (!(string.IsNullOrEmpty(t.codInstitucion)) && t.codInstitucion != "Null"))
            //    {

            //        Finish();

                     
            //        //        var newActivity = new Intent(this, typeof(RegistrarServicio));

            //        //        Bundle valuesForActivity = new Bundle();
            //        //        valuesForActivity.PutInt("ServiciosDet", t.ID);
            //        //        newActivity.PutExtras(valuesForActivity);

            //        //        //newActivity.PutExtra("ServiciosDet", t.ID);
            //        //        StartActivity(newActivity);
            //        //        return;
            //    }
            //}
            //    else
            //    {
            //        Toast.MakeText(this, "Servicio Finalizado", ToastLength.Long).Show();

            //        var regservicio = new RegistrarServicioModel
            //        {
            //            id_Solicitud = t.id_Solicitud,
            //            NumeroSolicitud = t.NumeroSolicitud,
            //            HoraEstado = string.Format("{0:HH:mm}", System.DateTime.Now),
            //            codEstado = "009",
            //            Estado = "C"
            //        };
            //        //actualizar localmente



            //        var servicio = new ServicioLocal();
            //        servicio.ID = t.ID;
            //        servicio.codEstado = regservicio.codEstado;
            //        servicio.HoraEstado = regservicio.HoraEstado;
            //        ServicioManager.SaveTask(servicio);



            //        var servicioDetalle = new ServicioItem();
            //        servicioDetalle.id_Solicitud = servicio.id_Solicitud;
            //        servicioDetalle.NumeroSolicitud = servicio.NumeroSolicitud;
            //        servicioDetalle.Nombre = servicio.nombrePaciente;
            //        servicioDetalle.Fecha = DateTime.Now;
            //        servicioDetalle.codMovil = t.codMovil;
            //        servicioDetalle.Estado = servicio.Estado;
            //        servicioDetalle.codEstado = servicio.codEstado;
            //        servicioDetalle.HoraEstado = servicio.HoraEstado;
            //        servicioDetalle.codInstitucion = regservicio.codInstitucion;
            //        servicioDetalle.codDesenlace = regservicio.codDesenlace;
            //        servicioDetalle.Enviado = false;
            //        servicioDetalle.AuditUsuario = user.nombres;
            //        servicioDetalle.AuditId = servicio.ID;
            //        servicioDetalle.GeoData = _locationText;
            //        servicioDetalle.Address = "";
            //        ServicioItemManager.SaveTask(servicioDetalle);

            //        await LoadServicios();
            //        return;
            //    }

            //}



            //if (t.codEstado != "002")
            //{
            var newActivity2 = new Intent(this, typeof(RegistrarServicio));

            Bundle valuesForActivity2 = new Bundle();
            valuesForActivity2.PutInt("ServiciosDet", t.ID);
            newActivity2.PutExtras(valuesForActivity2);

            //newActivity.PutExtra("ServiciosDet", t.ID);
            StartActivity(newActivity2);

            //}
            //else
            //{
            //var newActivity2 = new Intent(this, typeof(ServiciosDetalle));
            //// newActivity.PutExtra("ServiciosDet", t.ID);
            //Bundle valuesForActivity2 = new Bundle();
            //valuesForActivity2.PutInt("ServiciosDet", t.ID);
            //newActivity2.PutExtras(valuesForActivity2);
            //StartActivity(newActivity2);

            //}



        }



        private async Task LoadServicios()
        {
            string result;
            Toast.MakeText(this, "Buscando servicios...aguarde por favor...", ToastLength.Short).Show();
            string movil = user.codMovil;
            try
            {

                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;

                // client.BaseAddress = new Uri("http://181.120.121.221:88");
                client.BaseAddress = new Uri(IPCONN);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + session.getAccessToken());

                
                movil = movil.TrimEnd();
                // string url = string.Format("api/sas_ServiciosApi/{0}/{1}/{2}", user.codMovil.TrimEnd(), "001", "P");
                string url = string.Format("api/sas_ServiciosApi/00?idmovil={0}", movil);
                
                var response = await client.GetAsync(url);
                result = response.Content.ReadAsStringAsync().Result;
                //Items = JsonConvert.DeserializeObject <List<Personas>> (result);
                if ((response.ReasonPhrase== "Unauthorized"))
                {
                    Toast.MakeText(this, "La sesión a caducado, vuelva a ingresar", ToastLength.Long).Show();
                    session.logoutUser();
                    // StopService(new Intent("com.sas.searchpending"));
                    StopService(new Intent("com.xamarin.sas"));
                    Finish();
                    return;
                }

            }
            catch (Exception ex)
            {

                Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();

                return;
            }
            finally
            {

                servicios = ServicioManager.GetTasks(user.codMovil);

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

                ServicioLocalItem sl = new ServicioLocalItem();
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
                    //else
                    //{
                    //    var item = ServicioManager.GetTaskIdSol(t.id_Solicitud);
                    //    if (item.codEstado != t.codEstado)
                    //    {

                    //    }
                    //}
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
                servicios = ServicioManager.GetTasks(user.codMovil);
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

       
        private async void GetIndexDato(string codtabla)
        {
            
            string result;
            try
            {
                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;
                client.BaseAddress = new Uri(IPCONN);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + session.getAccessToken());
                string url = string.Format("api/SasDatosApi?idtabla={0}", codtabla);
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

        //        // string url = string.Format("api/sas_ServiciosApi/{0}/{1}/{2}", user.codMovil.TrimEnd(), "001", "P");
        //        string url = string.Format("api/sas_ServiciosApi/{0}/{1}/{2}", user.codMovil.TrimEnd(), "001", "P");

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