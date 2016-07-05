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

namespace sas.Actividades
{
    [Activity(Label = "Translado", Theme = "@style/MyCustomTheme", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class Translado : Activity, ILocationListener
    {

        EditText txtTA;
        EditText txtFC;
        EditText txtFResp;
        EditText txtSAT;
        EditText txtTemperatura;
        EditText txtglasgow;
        EditText txtGlicemia;
        EditText txtMedico;
        EditText txtDestinoDesenlace;

        Button btnGuardarTranslado;
        Button btnBuscar;
    
        
        TextView lblDescrpcionDestinoDesenlace;
        
        //ProgressBar mProgress;

        //manejo de sesion de usuario
        UserSessionManager session;
        string IPCONN = "";
        string usuario = "";
        string movil = "";

        //variables para gps
        string _addressText;
        Location _currentLocation;
        LocationManager _locationManager;
        string _locationProvider;
        string _locationText;
        static readonly string TAG = "X:" + typeof(RegistrarServicio).Name;

        //base de datos local
        private ServicioLocalItem servicio;
        private ServicioItem servicioDetalle;
        private int ID = 0;

        //bind service
        bool isBound = false;
        bool isConfigurationChange = false;
        DemoServiceBinder binder;
       
        DemoServiceConnection demoServiceConnection;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            try
            { 
            //asignar el diseño
                SetContentView(Resource.Layout.TransladoLayout);

                //recuperar IP para WEBAPI
                session = new UserSessionManager(this);
                IPCONN = session.getAccessConn();
                usuario = session.getAccessKey();
                movil = session.getAccessIdmovil();

                //asignar los controles del layout
                txtTA = FindViewById<EditText>(Resource.Id.txtTA);
                txtFC = FindViewById<EditText>(Resource.Id.txtFC);
                txtFResp = FindViewById<EditText>(Resource.Id.txtFresp);
                txtSAT = FindViewById<EditText>(Resource.Id.txtSAT);
                txtTemperatura = FindViewById<EditText>(Resource.Id.txtTemperatura);
                txtglasgow = FindViewById<EditText>(Resource.Id.txtglasgow);
                txtGlicemia = FindViewById<EditText>(Resource.Id.txtGlicemia);
                txtMedico = FindViewById<EditText>(Resource.Id.txtMedico);
                txtDestinoDesenlace = FindViewById<EditText>(Resource.Id.txtDestinoDesenlace);
                lblDescrpcionDestinoDesenlace = FindViewById<TextView>(Resource.Id.lblDescrpcionDestinoDesenlace);
                btnGuardarTranslado = FindViewById<Button>(Resource.Id.btnGuardarTranslado);
                btnBuscar = FindViewById<Button>(Resource.Id.btnBuscar);
                //mProgress = FindViewById<ProgressBar>(Resource.Id.mProgress);
                //mProgress.Visibility = ViewStates.Invisible;

               // asignar eventos
                btnGuardarTranslado.Click += BtnGuardarTranslado_Click;
                btnBuscar.Click += BtnBuscar_Click;

                //recibir datos de la actividad predecesora
                ID = Intent.Extras.GetInt("ServiciosDet");
                servicio = ServicioManager.GetTask(ID);

               // restore from connection there was a configuration change, such as a device rotation
                demoServiceConnection = LastNonConfigurationInstance as DemoServiceConnection;

                if (demoServiceConnection != null)
                    binder = demoServiceConnection.Binder;

            }
            catch (Exception ex)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Aviso");
                builder.SetMessage(ex.Message);
                builder.SetCancelable(false);
                builder.SetPositiveButton("OK", delegate { return; });
                builder.Show();

            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            try
            {
                var demoServiceIntent = new Intent("com.xamarin.sas");
                demoServiceConnection = new DemoServiceConnection(this);
                ApplicationContext.BindService(demoServiceIntent, demoServiceConnection, Bind.AutoCreate);
            }
            catch (Exception ex)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Aviso");
                builder.SetMessage(ex.Message);
                builder.SetCancelable(false);
                builder.SetPositiveButton("OK", delegate { return; });
                builder.Show();

            }

        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                // _locationManager = GetSystemService(Context.LocationService) as LocationManager;
                InitializeLocationManager();

                //if (_locationManager.IsProviderEnabled(LocationManager.GpsProvider))
                if ((!string.IsNullOrEmpty(_locationProvider)))
                {
                    _locationManager.RequestLocationUpdates(_locationProvider, 2000, 1, this);
                    Log.Debug(TAG, "Listening for location updates using " + _locationProvider + ".");
                }
                else
                {
                    //do nothing
                }
            }
            catch (Exception ex)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Aviso");
                builder.SetMessage(ex.Message);
                builder.SetCancelable(false);
                builder.SetPositiveButton("OK", delegate { return; });
                builder.Show();


            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!isConfigurationChange)
            {
                if (isBound)
                {
                    ApplicationContext.UnbindService(demoServiceConnection);
                    // UnbindService(demoServiceConnection);
                    isBound = false;
                }
            }
        }
        // return the service connection if there is a configuration change
        public override Java.Lang.Object OnRetainNonConfigurationInstance()
        {
            base.OnRetainNonConfigurationInstance();

            isConfigurationChange = true;

            return demoServiceConnection;
        }

        protected override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
            Log.Debug(TAG, "No longer listening for location updates.");
        }

        //resultado de la búsqueda
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                int idresul = data.Extras.GetInt("sasDatos");
                var SasdatosItem = SasDatosManager.GetTask(idresul);
                if (SasdatosItem != null)
                {
                    txtDestinoDesenlace.Text = SasdatosItem.codigo;
                    lblDescrpcionDestinoDesenlace.Text = SasdatosItem.descripcion;

                }
            }
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string codtabla = "";
            var newActivity = new Intent(this, typeof(Buscar));
            //newActivity.PutExtra("ServiciosDet", servicio);
            codtabla = "06";
            newActivity.PutExtra("codtabla", codtabla);
            StartActivityForResult(newActivity, 0);
        }



        private async void BtnGuardarTranslado_Click(object sender, EventArgs e)
        {
            //mProgress.Indeterminate = true;
            //mProgress.Visibility = ViewStates.Visible;

            await GetIndexDato(txtDestinoDesenlace.Text);

            if (string.IsNullOrEmpty(txtDestinoDesenlace.Text))
            {
               // Toast.MakeText(this, "Debe ingresar una institución", ToastLength.Long).Show();

                return;
            }


            //actualizar localmente
            servicio.ID = ID;
            servicio.codEstado = "005";
            servicio.HoraEstado = DateTime.Now.ToString("H:mm");
            servicio.Estado = servicio.Estado;
            servicio.sv_ta = txtTA.Text;
            servicio.sv_fc = txtFC.Text;
            servicio.sv_fresp = txtFResp.Text;
            servicio.sv_tempe = txtTemperatura.Text;
            servicio.SAT = txtSAT.Text;
            servicio.Glasgow = txtglasgow.Text;
            servicio.Glicemia = txtGlicemia.Text;
            servicio.IndicacionArribo = txtMedico.Text;
            servicio.codInstitucion = txtDestinoDesenlace.Text;
            ServicioManager.SaveTask(servicio);

            //GuardarDatos(regservicio);
            servicioDetalle = new ServicioItem();
            servicioDetalle.id_Solicitud = servicio.id_Solicitud;
            servicioDetalle.NumeroSolicitud = servicio.NumeroSolicitud;
            servicioDetalle.Nombre = servicio.nombrePaciente;
            servicioDetalle.Fecha = DateTime.Now;
            servicioDetalle.codMovil = movil;
            servicioDetalle.Estado = servicio.Estado;
            servicioDetalle.codEstado = servicio.codEstado;
            servicioDetalle.HoraEstado = servicio.HoraEstado;
            servicioDetalle.codInstitucion = servicio.codInstitucion;
            servicioDetalle.codDesenlace = servicio.codDesenlace;
            servicioDetalle.Enviado = false;
            servicioDetalle.AuditUsuario = usuario;
            servicioDetalle.AuditId = servicio.ID;
            servicioDetalle.GeoData = _locationText;
            servicioDetalle.sv_ta = txtTA.Text;
            servicioDetalle.sv_fc = txtFC.Text;
            servicioDetalle.sv_fresp = txtFResp.Text;
            servicioDetalle.sv_tempe = txtTemperatura.Text;
            servicioDetalle.SAT = txtSAT.Text;
            servicioDetalle.Glasgow = txtglasgow.Text;
            servicioDetalle.Glicemia = txtGlicemia.Text;
            servicioDetalle.IndicacionArribo = txtMedico.Text;
            servicioDetalle.codInstitucion = txtDestinoDesenlace.Text;
            ServicioItemManager.SaveTask(servicioDetalle);
            Toast.MakeText(this, "Registro guardado Correctamtne", ToastLength.Long).Show();

            if (isBound)
            {
                RunOnUiThread(() =>
                {
                    binder.GetDemoService().SincronizarEstados();
                    //string text = binder.GetDemoService().GetText();
                    //Console.WriteLine("{0} returned from DemoService", text);
                    //Toast.MakeText(this, string.Format( "{0} returned from DemoService", text), ToastLength.Long).Show();
                }

            );
            }

            //mProgress.Visibility = ViewStates.Gone;

            Intent intent = new Intent();
            intent.SetClass(BaseContext, typeof(Servicios));
            intent.SetFlags(ActivityFlags.ReorderToFront);
            StartActivity(intent);
            Finish();
        }


        class DemoServiceConnection : Java.Lang.Object, IServiceConnection
        {
            Translado activity;
            DemoServiceBinder binder;

            public DemoServiceBinder Binder
            {
                get
                {
                    return binder;
                }
            }

            public DemoServiceConnection(Translado activity)
            {
                this.activity = activity;
            }

            public void OnServiceConnected(ComponentName name, IBinder service)
            {
                var demoServiceBinder = service as DemoServiceBinder;
                if (demoServiceBinder != null)
                {
                    var binder = (DemoServiceBinder)service;
                    activity.binder = binder;
                    activity.isBound = true;

                    // keep instance for preservation across configuration changes
                    this.binder = (DemoServiceBinder)service;
                }
            }

            public void OnServiceDisconnected(ComponentName name)
            {
                activity.isBound = false;
            }
        }
        async Task GetIndexDato(string Id)
        {
            //string result;
            var busqueda = new SasDatosItem();
            try
            {
                //HttpClient client = new HttpClient();
                //client.MaxResponseContentBufferSize = 256000;
                //client.BaseAddress = new Uri(IPCONN);
                //var uri = new Uri (string.Format ("http://181.120.121.221:88/api/sas_ServiciosApi/{0}/{1}/{2}", deviceUser.codMovil,"001","P" ));
                string codtabla = "";
               
                    codtabla = "06";
               
                //string url = string.Format("/api/SasDatosApi?idtabla={0}&codigo={1}", codtabla, Id);
                //var response = await client.GetAsync(url);
                //result = response.Content.ReadAsStringAsync().Result;
                //Items = JsonConvert.DeserializeObject <List<Personas>> (result);

                //  busqueda = SasDatosManager.GetTask(1);

                busqueda = SasDatosManager.GetTaskTabCod(codtabla, Id);

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();
                //return;
            }
            //sasdatos = JsonConvert.DeserializeObject<List<SasDatosModel>>(result);

            if (busqueda.codigo != null)
            {
                lblDescrpcionDestinoDesenlace.Text = string.Format("{0}", busqueda.descripcion);
            }
            else
            {
                txtDestinoDesenlace.Text = string.Empty;
                lblDescrpcionDestinoDesenlace.Text = string.Empty;
                // institucionEntry.Focus();
                //await DisplayAlert("Error", "Código no existe", "Aceptar");
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Alerta");
                builder.SetMessage("Código no existe");
                builder.SetCancelable(false);
                builder.SetPositiveButton("OK", delegate { return; });
                builder.Show();

                // Toast.MakeText(this, "Código no existe", ToastLength.Long).Show();
            }


        }

        #region"GPS"

        //async void GetAddress()
        //{
        //    if (_currentLocation == null)
        //    {
        //        _addressText = "Can't determine the current address. Try again in a few minutes.";
        //        return;
        //    }

        //    Address address = await ReverseGeocodeCurrentLocation();
        //    DisplayAddress(address);
        //}

        async Task<Address> ReverseGeocodeCurrentLocation()
        {
            Address address;
            try
            {
                Geocoder geocoder = new Geocoder(this);
                IList<Address> addressList =
                    await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);

                address = addressList.FirstOrDefault();

            }
            catch
            {
                address = null;

            }
            return address;
        }

        void DisplayAddress(Address address)
        {
            if (address != null)
            {
                StringBuilder deviceAddress = new StringBuilder();
                for (int i = 0; i < address.MaxAddressLineIndex; i++)
                {
                    deviceAddress.AppendLine(address.GetAddressLine(i));
                }
                // Remove the last comma from the end of the address.
                _addressText = deviceAddress.ToString();
            }
            else
            {
                _addressText = "Unable to determine the address. Try again in a few minutes.";
            }
        }
        void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Coarse,
                PowerRequirement = Power.Medium


            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

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
        public async void OnLocationChanged(Location location)
        {
            _currentLocation = location;
            if (_currentLocation == null)
            {
                _locationText = "Unable to determine your location. Try again in a short while.";
            }
            else
            {
                _locationText = string.Format("{0:f6},{1:f6}", _currentLocation.Latitude, _currentLocation.Longitude);
                Address address = await ReverseGeocodeCurrentLocation();
                DisplayAddress(address);

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

        #endregion


        //#region "Menu"
        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    MenuInflater.Inflate(Resource.Menu.busqueda, menu);
        //    return base.OnCreateOptionsMenu(menu);
        //}
        //public override bool OnOptionsItemSelected(IMenuItem item)
        //{
        //    //Toast.MakeText(this, "Top ActionBar pressed: " + item.TitleFormatted, ToastLength.Short).Show();

        //    if (item.TitleFormatted.ToString() == "Buscar Institución")
        //    {
        //        string codtabla = "";
        //        var newActivity = new Intent(this, typeof(Buscar));
        //        //newActivity.PutExtra("ServiciosDet", servicio);
        //        codtabla = "06";
        //        newActivity.PutExtra("codtabla", codtabla);
        //        StartActivityForResult(newActivity, 0);

        //    }

        //   if (item.TitleFormatted.ToString() == "Buscar Médico")
        //    {


        //    }
        //    return base.OnOptionsItemSelected(item);
        //}
        //#endregion


    }
}