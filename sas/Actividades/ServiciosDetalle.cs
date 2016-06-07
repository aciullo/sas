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
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using sas.Core;
using sas.Clases;
using Android.Locations;
using Android.Util;

namespace sas
{
    [Activity(Label = "Servicios Detalle", Theme = "@style/MyCustomTheme")]
    public class ServiciosDetalle : Activity, ILocationListener
    {
        // private ServiciosModel servicio;
        private ServicioLocal servicio;
        private MotivosModel motivo;
        private int ID=0;
        EditText txtNroSolicitud;
        EditText txtNombrePaciente;
        EditText txtEdad;
        EditText txtSolicitante;
        EditText txtDireccion1;
        EditText txtMotivo1;
        EditText txtHoraLlamada;
        EditText txtTipo;
        Button btnIniciarServicio;

        //manejo de sesion de usuario
        UserSessionManager session;
        string usuario = "";

        //variables para gps
        string _addressText;
        Location _currentLocation;
        LocationManager _locationManager;
        string _locationProvider;
        string _locationText;
        static readonly string TAG = "X:" + typeof(ServiciosDetalle).Name;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.ServicesDetailLayout);

            //asignar los controles del layout
            txtNroSolicitud = FindViewById<EditText>(Resource.Id.txtNroSolicitud);
            txtNombrePaciente = FindViewById<EditText>(Resource.Id.txtNombrePaciente);
            txtEdad = FindViewById<EditText>(Resource.Id.txtEdad);
            txtSolicitante = FindViewById<EditText>(Resource.Id.txtSolicitante);
            txtDireccion1 = FindViewById<EditText>(Resource.Id.txtDireccion1);
            txtMotivo1 = FindViewById<EditText>(Resource.Id.txtMotivo1);
            txtHoraLlamada = FindViewById<EditText>(Resource.Id.txtHoraLlamada);
            txtTipo = FindViewById<EditText>(Resource.Id.txtTipo);
            btnIniciarServicio = FindViewById<Button>(Resource.Id.btnIniciarServicio);

            //   servicio = this.Intent.GetParcelableExtra("ServiciosDet") as ServiciosModel;
          //  ID = this.Intent.GetStringExtra("ServiciosDet");
            ID=  Intent.Extras.GetInt("ServiciosDet");
            servicio = ServicioManager.GetTask(ID);
            //recuperar usuario
            session = new UserSessionManager(this);
            usuario = session.getAccessKey();
            //mostrar datos
            txtNroSolicitud.Text = servicio.NumeroSolicitud.ToString();
            txtNombrePaciente.Text = servicio.nombrePaciente;
            txtEdad.Text = servicio.edadPaciente.ToString();
            txtSolicitante.Text = servicio.nombreSolicitante;
            txtDireccion1.Text = servicio.direccionReferecia + ", " + servicio.direccionReferecia2 + " Nro Casa" + servicio.numeroCasa;
            txtHoraLlamada.Text = servicio.hora_Llamado;
            txtTipo.Text = servicio.producto;
            //if (!string.IsNullOrEmpty(servicio.codMotivo1))
            //{
            //    await LoadMotivo1();
            //}
            //if (!string.IsNullOrEmpty(servicio.codMotivo2))
            //{
            //    await LoadMotivo2();
            //}
            //if (!string.IsNullOrEmpty(servicio.codMotivo3))
            //{
            //    await LoadMotivo3();
            //}
            string cadenamotivo = (!string.IsNullOrEmpty(servicio.codMotivo1)) ? servicio.codMotivo1 : string.Empty ;
            cadenamotivo = cadenamotivo + " " + ((!string.IsNullOrEmpty(servicio.codMotivo2)) ? "," + servicio.codMotivo2 : string.Empty);
            cadenamotivo = cadenamotivo + " " + ((!string.IsNullOrEmpty(servicio.codMotivo3)) ? "," + servicio.codMotivo3 : string.Empty);
            cadenamotivo = cadenamotivo + " " + ((!string.IsNullOrEmpty(servicio.OtroMotivo)) ? "," + servicio.OtroMotivo : string.Empty);
            txtMotivo1.Text = cadenamotivo;
            btnIniciarServicio.Click += BtnIniciarServicio_Click;
        }

        private void BtnIniciarServicio_Click(object sender, EventArgs e)
        {
            //registrar salida de base
            servicio.ID = ID;
            servicio.Estado = servicio.Estado;
            servicio.HoraEstado = servicio.HoraEstado;
            servicio.codEstado = "003";
            ServicioManager.SaveTask(servicio);

            var servicioDetalle = new ServicioItem();
            servicioDetalle.id_Solicitud = servicio.id_Solicitud;
            servicioDetalle.NumeroSolicitud = servicio.NumeroSolicitud;
            servicioDetalle.Nombre = servicio.nombrePaciente;
            servicioDetalle.Fecha = DateTime.Now;
            servicioDetalle.codMovil = servicio.codMovil;
            servicioDetalle.Estado = servicio.Estado;
            servicioDetalle.codEstado = servicio.codEstado;
            servicioDetalle.HoraEstado = servicio.HoraEstado;
            servicioDetalle.codInstitucion = servicio.codInstitucion;
            servicioDetalle.codDesenlace = servicio.codDesenlace;
            servicioDetalle.Enviado = false;
            servicioDetalle.AuditUsuario = usuario;
            servicioDetalle.AuditId = servicio.ID;
            servicioDetalle.GeoData = _locationText;
            ServicioItemManager.SaveTask(servicioDetalle);



            var newActivity = new Intent(this, typeof(RegistrarServicio));
            // newActivity.PutExtra("ServiciosDet", servicio.ID);
            Bundle valuesForActivity = new Bundle();
            valuesForActivity.PutInt("ServiciosDet", ID);
            newActivity.PutExtras(valuesForActivity);
            StartActivity(newActivity);
            Finish();
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


        protected override void OnResume()
        {
            base.OnResume();

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

                //AlertDialog.Builder builder = new AlertDialog.Builder(this);
                //builder.SetTitle("Location Services Not Active");
                //builder.SetMessage("Please enable Location Services and GPS");
                //builder.SetPositiveButton("OK", delegate
                //{
                //    // Show location settings when the user acknowledges the alert dialog
                //    Intent intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                //    StartActivity(intent);

                //});
                //builder.SetCancelable(true);
                //builder.SetCancelButton("Cancelar", delegate { return; });
                //Dialog alertDialog = builder.Create();
                //alertDialog.SetCanceledOnTouchOutside(false);
                //alertDialog.Show();

            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
            Log.Debug(TAG, "No longer listening for location updates.");
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

        //private async Task LoadMotivo1()
        //{
        //    string result;


        //    try
        //    {

        //        HttpClient client = new HttpClient();
        //        client.MaxResponseContentBufferSize = 256000;

        //        client.BaseAddress = new Uri("http://181.120.121.221:88");
        //        //var uri = new Uri (string.Format ("http://181.120.121.221:88/api/sas_ServiciosApi/{0}/{1}/{2}", deviceUser.codMovil,"001","P" ));

        //        string url = string.Format("/api/MotivosModelsApi/{0}", servicio.codMotivo1.TrimEnd());
        //        var response = await client.GetAsync(url);
        //        result = response.Content.ReadAsStringAsync().Result;
        //        //Items = JsonConvert.DeserializeObject <List<Personas>> (result);

        //    }
        //    catch (Exception ex)
        //    {
        //        Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();

        //        return;
        //    }

        //    try
        //    {

        //        motivo = JsonConvert.DeserializeObject<MotivosModel>(result);
        //        txtMotivo1.Text = txtMotivo1.Text + ", " + motivo.descripcionMotivo;
        //        //motivosListView.ItemsSource = motivo;
        //        //se carga el picker recorriendo


        //    }
        //    catch (Exception ex)
        //    {
        //        Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();

        //        return;
        //    }


        //}


        //private async  Task LoadMotivo2()
        //{
        //    string result;


        //    try
        //    {

        //        HttpClient client = new HttpClient();
        //        client.MaxResponseContentBufferSize = 256000;

        //        client.BaseAddress = new Uri("http://181.120.121.221:88");
        //        //var uri = new Uri (string.Format ("http://181.120.121.221:88/api/sas_ServiciosApi/{0}/{1}/{2}", deviceUser.codMovil,"001","P" ));

        //        string url = string.Format("/api/MotivosModelsApi/{0}", servicio.codMotivo2.TrimEnd());
        //        var response = await client.GetAsync(url);
        //        result = response.Content.ReadAsStringAsync().Result;
        //        //Items = JsonConvert.DeserializeObject <List<Personas>> (result);

        //    }
        //    catch (Exception ex)
        //    {
        //        Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();
        //        return;
        //    }

        //    try
        //    {

        //        motivo = JsonConvert.DeserializeObject<MotivosModel>(result);
        //        txtMotivo1.Text = txtMotivo1.Text + ", " + motivo.descripcionMotivo;
        //        //motivosListView.ItemsSource = motivo;
        //        //se carga el picker recorriendo


        //    }
        //    catch (Exception ex)
        //    {

        //        Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
        //        return;
        //    }


        //}


        //private async Task LoadMotivo3()
        //{
        //    string result;


        //    try
        //    {

        //        HttpClient client = new HttpClient();
        //        client.MaxResponseContentBufferSize = 256000;

        //        client.BaseAddress = new Uri("http://181.120.121.221:88");
        //        //var uri = new Uri (string.Format ("http://181.120.121.221:88/api/sas_ServiciosApi/{0}/{1}/{2}", deviceUser.codMovil,"001","P" ));

        //        string url = string.Format("/api/MotivosModelsApi/{0}", servicio.codMotivo3.TrimEnd());
        //        var response = await client.GetAsync(url);
        //        result = response.Content.ReadAsStringAsync().Result;
        //        //Items = JsonConvert.DeserializeObject <List<Personas>> (result);

        //    }
        //    catch (Exception ex)
        //    {
        //        Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();

        //        return;
        //    }

        //    try
        //    {

        //        motivo = JsonConvert.DeserializeObject<MotivosModel>(result);
        //        txtMotivo1.Text = txtMotivo1.Text + ", " + motivo.descripcionMotivo;
        //        //motivosListView.ItemsSource = motivo;
        //        //se carga el picker recorriendo


        //    }
        //    catch (Exception ex)
        //    {

        //        Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
        //        return;
        //    }


        //}
    }
}