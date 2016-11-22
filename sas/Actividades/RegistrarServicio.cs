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

namespace sas
{
    [Activity(Label = "Registrar Servicio", Theme = "@style/MyCustomTheme", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class RegistrarServicio : Activity, ILocationListener
    {
        //clases y variables
        // private ServiciosModel servicio;
        static readonly string TAG = "X:" + typeof(RegistrarServicio).Name;
        //base de datos local
        private ServicioLocalItem servicio;
        private ServicioItem servicioDetalle;
        private int ID = 0;
        //private List<SasDatosModel> sasdatos;
        //private SasDatosModel sasdatosBusqueda;
        //variables para gps
        string _addressText;
        Location _currentLocation;
        LocationManager _locationManager;
        string _locationProvider;
        string _locationText;
        //controles de la vista
        EditText txtNroSolicitud;
        EditText txtNombrePaciente;
        EditText txtEdad;

        EditText txtDireccion1;
        EditText txtMotivo1;
        EditText txtTipo;

        Button btnRegistroInicial;
        Button btnVolverBase;
        Button btnTranslado;
        Button btnRegistrarResultado;
        Button btnBuscar;
        Button btnRegistroLocal;
        TextView lblDestinoDesenlace;
        TextView lblDescrpcionDestinoDesenlace;
        TextView txtaddress;
        EditText txtDestinoDesenlace;
        ProgressBar mProgress;

        //manejo de sesion de usuario
        UserSessionManager session;
        string IPCONN = "";
        string usuario = "";
        string movil = "";

        private string codEstadoRecibido = "";
        private string codInstitucionRecibido = "";
        private string codDesenlace = "";

        //bind service
        bool isBound = false;
        bool isConfigurationChange = false;
        DemoServiceBinder binder;
        DemoServiceConnection demoServiceConnection;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            //asignar el diseño
            SetContentView(Resource.Layout.RegistrarServiciolayout);

            //recuperar IP para WEBAPI
            session = new UserSessionManager(this);
            IPCONN = session.getAccessConn();
            usuario = session.getAccessKey();
            movil = session.getAccessIdmovil();

            //asignar los controles del layout
            txtNroSolicitud = FindViewById<EditText>(Resource.Id.txtNroSolicitud);
            txtNombrePaciente = FindViewById<EditText>(Resource.Id.txtNombrePaciente);
            txtEdad = FindViewById<EditText>(Resource.Id.txtEdad);
            btnRegistroInicial = FindViewById<Button>(Resource.Id.btnRegistroInicial);
            btnVolverBase = FindViewById<Button>(Resource.Id.btnVolverBase);
            btnTranslado = FindViewById<Button>(Resource.Id.btnTranslado);
            btnRegistrarResultado = FindViewById<Button>(Resource.Id.btnRegistrarResultado);
            lblDestinoDesenlace = FindViewById<TextView>(Resource.Id.lblDestinoDesenlace);
            lblDescrpcionDestinoDesenlace = FindViewById<TextView>(Resource.Id.lblDescrpcionDestinoDesenlace);
            txtDestinoDesenlace = FindViewById<EditText>(Resource.Id.txtDestinoDesenlace);
           mProgress = FindViewById<ProgressBar>(Resource.Id.mProgress);
           mProgress.Visibility = ViewStates.Invisible;
            btnBuscar = FindViewById<Button>(Resource.Id.btnBuscar);
            btnRegistroLocal = FindViewById<Button>(Resource.Id.btnRegistroLocal);

            txtDireccion1 = FindViewById<EditText>(Resource.Id.txtDireccion1);
            txtMotivo1 = FindViewById<EditText>(Resource.Id.txtMotivo1);
            txtTipo = FindViewById<EditText>(Resource.Id.txtTipo);

            txtaddress = FindViewById<TextView>(Resource.Id.textView8);
            //recibir datos de la actividad predecesora
            ID = Intent.Extras.GetInt("ServiciosDet");
            servicio = ServicioManager.GetTask(ID);

            //mostrar los datos recibidos
            txtNroSolicitud.Text = servicio.NumeroSolicitud.ToString();
            txtNombrePaciente.Text = servicio.nombrePaciente;
            txtEdad.Text = servicio.edadPaciente.ToString();
            txtDireccion1.Text = servicio.direccionReferecia.ToString();
            txtTipo.Text = servicio.producto.ToString();
            string cadenamotivo = (!string.IsNullOrEmpty(servicio.codMotivo1)) ? servicio.codMotivo1 : string.Empty;
            cadenamotivo = cadenamotivo + " " + ((!string.IsNullOrEmpty(servicio.codMotivo2)) ? "," + servicio.codMotivo2 : string.Empty);
            cadenamotivo = cadenamotivo + " " + ((!string.IsNullOrEmpty(servicio.codMotivo3)) ? "," + servicio.codMotivo3 : string.Empty);
            cadenamotivo = cadenamotivo + " " + ((!string.IsNullOrEmpty(servicio.OtroMotivo)) ? "," + servicio.OtroMotivo : string.Empty);
            txtMotivo1.Text = cadenamotivo;

            //variables
            codEstadoRecibido =  servicio.codEstado;
            codInstitucionRecibido = servicio.codInstitucion;
            codDesenlace = servicio.codDesenlace;

           

            //asignar los eventos a los controles
            btnRegistroInicial.Click += BtnRegistroInicial_Click;
            btnVolverBase.Click += BtnVolverBase_Click;
            btnRegistrarResultado.Click += BtnRegistrarResultado_Click;
            btnTranslado.Click += BtnTranslado_Click;
            btnRegistroLocal.Click += BtnRegistroLocal_Click;
            txtDestinoDesenlace.KeyPress += TxtDestinoDesenlace_KeyPress;
            txtDestinoDesenlace.FocusChange += TxtDestinoDesenlace_FocusChange;
            btnBuscar.Click += BtnBuscar_Click;
            //  GetAddress();

            // restore from connection there was a configuration change, such as a device rotation
            demoServiceConnection = LastNonConfigurationInstance as DemoServiceConnection;

            if (demoServiceConnection != null)
                binder = demoServiceConnection.Binder;

            InitializeLocationManager();

            //mostrar botones segun ÚLTIMO estado
            LoadStateButtons(codEstadoRecibido);

        }

        protected override void OnStart()
        {
            base.OnStart();

            var demoServiceIntent = new Intent("com.xamarin.sas");
            demoServiceConnection = new DemoServiceConnection(this);
            ApplicationContext.BindService(demoServiceIntent, demoServiceConnection, Bind.AutoCreate);


           
        }

        protected override void OnResume()
        {
            base.OnResume();

            // _locationManager = GetSystemService(Context.LocationService) as LocationManager;
            //InitializeLocationManager();

            //if (_locationManager.IsProviderEnabled(LocationManager.GpsProvider))
            if ((!string.IsNullOrEmpty(_locationProvider)))
            {
                //mProgress.Indeterminate = true;
                //mProgress.Visibility = ViewStates.Visible;
             
                //txtaddress.Text = "Buscando Ubicación";
                _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
                Log.Debug(TAG, "Listening for location updates using " + _locationProvider + ".");
            }
            else
            {
                //do nothing
            }

            //var demoServiceIntent = new Intent("com.xamarin.sas");
            //demoServiceConnection = new DemoServiceConnection(this);
            //ApplicationContext.BindService(demoServiceIntent, demoServiceConnection, Bind.AutoCreate);
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
                txtaddress.Text = _addressText;
                 // mProgress.Visibility = ViewStates.Gone;
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
                Accuracy = Accuracy.Fine


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
           // throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
           // throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Log.Debug(TAG, "{0}, {1}", provider, status);
        }

        #endregion


        private void BtnRegistroLocal_Click(object sender, EventArgs e)
        {
            var newActivity = new Intent(this, typeof(RegistrarServicioLocal));
            StartActivity(newActivity);
        }

        //resultado de la búsqueda
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
               int idresul = data.Extras.GetInt("sasDatos");
               var  SasdatosItem  = SasDatosManager.GetTask(idresul);
               if (SasdatosItem != null)
                {
                    txtDestinoDesenlace.Text = SasdatosItem.codigo;
                    lblDescrpcionDestinoDesenlace.Text = SasdatosItem.descripcion;

                }
            }

        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string codtabla="";
            var newActivity = new Intent(this, typeof(Buscar));
            //newActivity.PutExtra("ServiciosDet", servicio);
            if (lblDestinoDesenlace.Text == "Desenlace")
            {
                codtabla = "07";
            }
            else
            {
                codtabla = "06";
            }
            newActivity.PutExtra("codtabla", codtabla);
            StartActivityForResult(newActivity,0);
           // StartActivity(newActivity);
        }

        private async void LoadStateButtons(string codEstadoRecibido)
        {
            switch (codEstadoRecibido)
            {
              
                case ("002"):
                    btnRegistroInicial.Text = "Registrar Salida de Base";
                    btnVolverBase.Visibility = ViewStates.Invisible;
                    btnTranslado.Visibility = ViewStates.Invisible;
                    break;
                case "003":
                    btnRegistroInicial.Text = "Registrar Llegada a Servicio";
                    btnVolverBase.Visibility = ViewStates.Invisible;
                    btnTranslado.Visibility = ViewStates.Invisible;
                    break;
                case "004":
                    btnRegistroInicial.Enabled = false;
                    btnRegistroInicial.Visibility = ViewStates.Invisible;

                    btnVolverBase.Visibility = ViewStates.Visible;
                    btnTranslado.Visibility = ViewStates.Visible;

                    btnVolverBase.Enabled = true;
                    btnTranslado.Enabled = true;

                    if (!string.IsNullOrEmpty(codDesenlace))
                    {
                        btnRegistroInicial.Text = "Registrar Salida a Base";
                        btnRegistroInicial.Enabled = true;
                        btnRegistroInicial.Visibility = ViewStates.Visible;
                        btnRegistrarResultado.Visibility = ViewStates.Invisible;

                        txtDestinoDesenlace.Visibility = ViewStates.Invisible;
                        lblDestinoDesenlace.Visibility = ViewStates.Invisible;
                        txtDestinoDesenlace.Visibility = ViewStates.Invisible;
                        btnBuscar.Visibility = ViewStates.Invisible;
                        btnVolverBase.Visibility = ViewStates.Invisible;
                        btnTranslado.Visibility = ViewStates.Invisible;

                    }
                    //else
                    //{
                    //    btnRegistroInicial.Text = "Registrar Salida a Base";
                    //    btnRegistroInicial.Enabled = true;
                    //    btnRegistroInicial.Visibility = ViewStates.Visible;
                    //    btnRegistrarResultado.Visibility = ViewStates.Invisible;

                    //    txtDestinoDesenlace.Visibility = ViewStates.Invisible;
                    //    lblDestinoDesenlace.Visibility = ViewStates.Invisible;
                    //    txtDestinoDesenlace.Visibility = ViewStates.Invisible;
                    //    btnBuscar.Visibility = ViewStates.Invisible;
                    //    btnVolverBase.Visibility = ViewStates.Visible;
                    //    btnTranslado.Visibility = ViewStates.Visible;

                    //}

                    break;
                case "005":
                    // btnRegistroInicial.Enabled = false;
                    btnVolverBase.Enabled = false;
                    btnTranslado.Enabled = false;
                    btnRegistrarResultado.Visibility = ViewStates.Visible;
                    btnRegistroInicial.Visibility = ViewStates.Invisible;
                    btnVolverBase.Visibility = ViewStates.Invisible;
                    btnTranslado.Visibility = ViewStates.Invisible;

                    btnRegistrarResultado.Visibility = ViewStates.Invisible;
                    if ( (string.IsNullOrEmpty(codInstitucionRecibido) || codInstitucionRecibido=="Null") &&  string.IsNullOrEmpty(codDesenlace) )
                    {
                        btnRegistrarResultado.Text = "Registrar Desenlace";
                        txtDestinoDesenlace.Visibility = ViewStates.Visible;
                        lblDestinoDesenlace.Visibility = ViewStates.Visible;
                        txtDestinoDesenlace.Visibility = ViewStates.Visible;
                        btnBuscar.Visibility = ViewStates.Visible;
                        txtDestinoDesenlace.Enabled = true;
                        lblDestinoDesenlace.Text = "Desenlace";
                        txtDestinoDesenlace.Text = string.Empty;
                        lblDescrpcionDestinoDesenlace.Text = string.Empty;
                        return;
                    }
                  

                    
                        if ((!string.IsNullOrEmpty(codInstitucionRecibido) && codInstitucionRecibido != "Null") )
                    {
                        txtDestinoDesenlace.Visibility = ViewStates.Visible;
                        lblDestinoDesenlace.Visibility = ViewStates.Visible;
                        txtDestinoDesenlace.Visibility = ViewStates.Visible;
                        btnBuscar.Visibility = ViewStates.Invisible;
                        //mostrar institucion
                        txtDestinoDesenlace.Text = codInstitucionRecibido;
                        await GetIndexDato(txtDestinoDesenlace.Text);
                        btnRegistrarResultado.Text = "Registrar Llegada a Institución";
                        txtDestinoDesenlace.Enabled = false;
                        btnRegistrarResultado.Visibility = ViewStates.Visible;
                        btnRegistrarResultado.Enabled = true;
                        return;
                    }

                    if (!string.IsNullOrEmpty(codDesenlace) && ( string.IsNullOrEmpty(codInstitucionRecibido) || codInstitucionRecibido == "Null"))
                    {

                        btnRegistroInicial.Enabled = true;
                        btnVolverBase.Enabled = false;
                        btnTranslado.Enabled = false;
                        btnRegistrarResultado.Visibility = ViewStates.Invisible;
                        btnVolverBase.Visibility = ViewStates.Invisible;
                        btnTranslado.Visibility = ViewStates.Invisible;
                        btnRegistroInicial.Visibility = ViewStates.Visible;
                        btnRegistroInicial.Text = "Registrar Llegada a Base";
                    }
                    break;
                case "006":

                    btnRegistrarResultado.Text = "Registrar Salida de Institución";
                    btnRegistrarResultado.Visibility = ViewStates.Visible;
                    btnRegistrarResultado.Enabled = true;

                    btnRegistroInicial.Enabled = true;
                    btnVolverBase.Enabled = false;
                    btnTranslado.Enabled = false;
                    btnRegistroInicial.Visibility = ViewStates.Invisible;
                    btnVolverBase.Visibility = ViewStates.Invisible;
                    btnTranslado.Visibility = ViewStates.Invisible;
                   
                    txtDestinoDesenlace.Visibility = ViewStates.Visible;
                    lblDestinoDesenlace.Visibility = ViewStates.Visible;
                    txtDestinoDesenlace.Visibility = ViewStates.Visible;
                    btnBuscar.Visibility = ViewStates.Visible;
                    lblDestinoDesenlace.Text = "Institución";
                    txtDestinoDesenlace.Text = codInstitucionRecibido;
                    await GetIndexDato(txtDestinoDesenlace.Text);
                    txtDestinoDesenlace.Enabled = false;
                    btnBuscar.Visibility = ViewStates.Invisible;
                    break;
                case "007":
                    btnRegistroInicial.Enabled = true;
                    btnRegistroInicial.Visibility = ViewStates.Visible;
                    btnRegistroInicial.Text = "Registrar Llegada a Base";

                    txtDestinoDesenlace.Visibility = ViewStates.Invisible;
                    lblDestinoDesenlace.Visibility = ViewStates.Invisible;
                    txtDestinoDesenlace.Visibility = ViewStates.Invisible;
                    btnBuscar.Visibility = ViewStates.Invisible;
                    btnRegistrarResultado.Enabled = true;
                    btnVolverBase.Enabled = false;
                    btnTranslado.Enabled = false;
                    btnRegistrarResultado.Visibility = ViewStates.Invisible;
                    btnVolverBase.Visibility = ViewStates.Invisible;
                    btnTranslado.Visibility = ViewStates.Invisible;
                  
                    break;
                case "008":
                  
                  
                  
                    if (!(string.IsNullOrEmpty(codInstitucionRecibido))  && codInstitucionRecibido != "Null")
                    {
                        btnRegistrarResultado.Text = "Registrar Desenlace";
                        btnRegistrarResultado.Visibility = ViewStates.Visible;
                        btnRegistrarResultado.Enabled = true;

                        btnRegistroInicial.Visibility = ViewStates.Invisible;
                        btnVolverBase.Visibility = ViewStates.Invisible;
                        btnTranslado.Visibility = ViewStates.Invisible;
                     

                        txtDestinoDesenlace.Visibility = ViewStates.Visible;
                        lblDestinoDesenlace.Visibility = ViewStates.Visible;
                        txtDestinoDesenlace.Visibility = ViewStates.Visible;
                        btnBuscar.Visibility = ViewStates.Visible;
                        txtDestinoDesenlace.Enabled = true;
                        lblDestinoDesenlace.Text = "Desenlace";
                        txtDestinoDesenlace.Text = string.Empty;
                        lblDescrpcionDestinoDesenlace.Text = string.Empty;
                    }
                    else
                    {
                        //conectar servicio
                        var demoServiceIntent = new Intent("com.xamarin.sas");
                        demoServiceConnection = new DemoServiceConnection(this);
                        ApplicationContext.BindService(demoServiceIntent, demoServiceConnection, Bind.AutoCreate);

                        Toast.MakeText(this, "Servicio Finalizado", ToastLength.Long).Show();
                       
                       var regservicio = new RegistrarServicioModel
                        {
                            id_Solicitud = servicio.id_Solicitud,
                            NumeroSolicitud = servicio.NumeroSolicitud,
                            HoraEstado = string.Format("{0:HH:mm}", System.DateTime.Now),
                            codEstado = "009",
                            Estado = "C"
                        };
                        //actualizar localmente
                        servicio.ID = ID;
                        servicio.codEstado = regservicio.codEstado;
                        servicio.Estado = regservicio.Estado;
                        servicio.HoraEstado = regservicio.HoraEstado;
                        ServicioManager.SaveTask(servicio);
                        GuardarDatos(regservicio);

                        //si es el ultimo evento
                        Intent i = new Intent(BaseContext, typeof(Servicios));
                        Bundle valuesForActivity = new Bundle();
                        valuesForActivity.PutInt("GPS", 1);
                        i.PutExtras(valuesForActivity);
                        // Closing all the Activities
                        i.SetFlags(ActivityFlags.ClearTask);

                        // Add new Flag to start new Activity
                        i.SetFlags(ActivityFlags.NewTask);

                        // Staring service Activity
                        BaseContext.StartActivity(i);


                        Finish();
                        //Intent intent = new Intent();
                        //intent.SetClass(BaseContext, typeof(Servicios));
                        //intent.SetFlags(ActivityFlags.ReorderToFront);
                        //StartActivity(intent);
                        //Finish();
                    }
                    break;

            }
        }

        private async void TxtDestinoDesenlace_KeyPress(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                //Toast.MakeText(this, txtDestinoDesenlace.Text, ToastLength.Short).Show();
               await GetIndexDato(txtDestinoDesenlace.Text);
                e.Handled = true;
            }
        }

        private async void TxtDestinoDesenlace_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                //Toast.MakeText(this, txtDestinoDesenlace.Text, ToastLength.Short).Show();
                 await GetIndexDato(txtDestinoDesenlace.Text);

            }
        }

        private void BtnTranslado_Click(object sender, EventArgs e)
        {
            //btnVolverBase.Enabled = false;
            //btnTranslado.Enabled = false;
            //txtDestinoDesenlace.Enabled = true;
            //btnRegistroInicial.Enabled = true;
            //btnRegistrarResultado.Text = "Registrar Salida a Institución";
            //btnRegistrarResultado.Visibility = ViewStates.Visible;
            //btnRegistroInicial.Visibility = ViewStates.Invisible;
            //btnVolverBase.Visibility = ViewStates.Invisible;
            //btnTranslado.Visibility = ViewStates.Invisible;

            //btnBuscar.Visibility = ViewStates.Visible;
            //txtDestinoDesenlace.Visibility = ViewStates.Visible;
            //txtDestinoDesenlace.Visibility = ViewStates.Visible;
            //lblDestinoDesenlace.Visibility = ViewStates.Visible;

            //lblDestinoDesenlace.Text = "Institución";
            //txtDestinoDesenlace.Text = string.Empty;
            //lblDescrpcionDestinoDesenlace.Text = string.Empty;

            var newActivity = new Intent(this, typeof(Translado));

            Bundle valuesForActivity = new Bundle();
            valuesForActivity.PutInt("ServiciosDet", ID);
            newActivity.PutExtras(valuesForActivity);

            StartActivity(newActivity);
            Finish();
        }

        private void BtnVolverBase_Click(object sender, EventArgs e)
        {
    
    
            btnRegistrarResultado.Visibility= ViewStates.Visible;
            btnRegistroInicial.Visibility = ViewStates.Invisible;
            btnVolverBase.Visibility = ViewStates.Invisible;
            btnTranslado.Visibility = ViewStates.Invisible;


            btnRegistrarResultado.Text = "Registrar Desenlace";
            txtDestinoDesenlace.Visibility = ViewStates.Visible;
            lblDestinoDesenlace.Visibility = ViewStates.Visible;
            txtDestinoDesenlace.Visibility = ViewStates.Visible;
            btnBuscar.Visibility = ViewStates.Visible;
            txtDestinoDesenlace.Enabled = true;
            lblDestinoDesenlace.Text = "Desenlace";
            txtDestinoDesenlace.Text = string.Empty;
            lblDescrpcionDestinoDesenlace.Text = string.Empty;
        }

        private void BtnRegistroInicial_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Confirmación");
            builder.SetMessage("Se encuetra a punto de actualizar el servicio. ¿Continuar?.");
            builder.SetCancelable(true);
            builder.SetPositiveButton("Si", delegate
            {
                //mProgress.Indeterminate = true;
                // mProgress.Visibility = ViewStates.Visible;
              
                GuardarEstadoInicial();
             });
            builder.SetNegativeButton("No", delegate { return; });
            builder.Show();
         
        }

        private void GuardarEstadoInicial()
        {
            string idestado = "";
            switch (btnRegistroInicial.Text)
            {
                case "Registrar Salida de Base":
                    idestado = "003";
                    // btnRegistroInicial.BackgroundTintList= Color.Navy;
                    btnRegistroInicial.Text = "Registrar Llegada a Servicio";
                    break;
                case "Registrar Llegada a Servicio":
                    idestado = "004";
                    btnRegistroInicial.Enabled = false;
                    btnVolverBase.Enabled = true;
                    btnTranslado.Enabled = true;
                    btnRegistroInicial.Visibility = ViewStates.Invisible;
                    btnVolverBase.Visibility = ViewStates.Visible;
                    btnTranslado.Visibility = ViewStates.Visible;
                    break;
                case "Registrar Salida a Base":
                    idestado = "005";
                    //volverBaseButton.BackgroundColor = Color.Green;
                    // btnRegistrarResultado.Text = "Servicio Finalizado";
                    btnRegistroInicial.Text = "Registrar Llegada a Base";
                    break;
                case "Registrar Llegada a Base":
                    idestado = "008";

                    break;
                default:
                    idestado = "";
                    break;
            }

            if (!string.IsNullOrEmpty(idestado))
            {
                var regservicio = new RegistrarServicioModel
                {
                    id_Solicitud = servicio.id_Solicitud,
                    NumeroSolicitud = servicio.NumeroSolicitud,
                    HoraEstado = string.Format("{0:HH:mm}", System.DateTime.Now),
                    codEstado = idestado,
                    Estado = servicio.Estado
                };
                //actualizar localmente
                servicio.ID = ID;
                servicio.codEstado = regservicio.codEstado;
                servicio.HoraEstado = regservicio.HoraEstado;
                ServicioManager.SaveTask(servicio);

             

                GuardarDatos(regservicio);



                //if (idestado == "008")
                //{
                //    regservicio = new RegistrarServicioModel
                //    {
                //        id_Solicitud = servicio.id_Solicitud,
                //        NumeroSolicitud = servicio.NumeroSolicitud,
                //        HoraEstado = string.Format("{0:HH:mm}", System.DateTime.Now),
                //        codEstado = "009",
                //        Estado = "C"
                //    };
                //    //actualizar localmente
                //    servicio.ID = ID;
                //    servicio.codEstado = regservicio.codEstado;
                //    servicio.HoraEstado = regservicio.HoraEstado;
                //    ServicioManager.SaveTask(servicio);
                //    await GuardarDatos(regservicio);

                //}
                //mProgress.Visibility = ViewStates.Gone;
                Intent intent = new Intent();
                intent.SetClass(BaseContext, typeof(Servicios));
                intent.SetFlags(ActivityFlags.ReorderToFront);
                StartActivity(intent);
                Finish();

            }
        }


        private void BtnRegistrarResultado_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Confirmación");
            builder.SetMessage("Se encuetra a punto de actualizar el servicio. ¿Continuar?.");
            builder.SetCancelable(true);
            builder.SetPositiveButton("Si", delegate
            {

                //mProgress.Indeterminate = true;
                //  mProgress.Visibility = ViewStates.Visible;
       
                GuardarResultados();

            });
            builder.SetNegativeButton("No", delegate { return; });
            builder.Show();
        }


        private async void GuardarResultados()
        {
            string idestado = "";

            switch (btnRegistrarResultado.Text)
            {
                case "Registrar Salida a Base":
                    idestado = "005";
                    //volverBaseButton.BackgroundColor = Color.Green;
                    // btnRegistrarResultado.Text = "Servicio Finalizado";
                    btnRegistrarResultado.Text = "Registrar Llegada a Base";
                    break;

                //case "Servicio Finalizado":
                //    idestado = "009";
                //    //volverBaseButton.BackgroundColor = Color.Purple;
                //    btnRegistrarResultado.Text = "Registrar Llegada a Base";

                //    break;
                case "Registrar Llegada a Base":
                    idestado = "008";
                    // volverBaseButton.BackgroundColor = Color.Maroon;
                    //btnRegistrarResultado.Text = "Registrar Desenlace";
                    //txtDestinoDesenlace.Visibility = ViewStates.Visible;
                    //lblDestinoDesenlace.Visibility = ViewStates.Visible;
                    //txtDestinoDesenlace.Visibility = ViewStates.Visible;
                    //btnBuscar.Visibility = ViewStates.Visible;
                    //txtDestinoDesenlace.Enabled = true;
                    //lblDestinoDesenlace.Text = "Desenlace";
                    //txtDestinoDesenlace.Text = string.Empty;
                    //lblDescrpcionDestinoDesenlace.Text = string.Empty;
                    //volverBaseButton.IsEnabled = false;
                    break;
                case "Registrar Desenlace":
                    await GetIndexDato(txtDestinoDesenlace.Text);
                    //idestado = "009";
                    if (string.IsNullOrEmpty(txtDestinoDesenlace.Text))
                    {
                        Toast.MakeText(this, "Debe ingresar un desenlace", ToastLength.Long).Show();
                       // mProgress.Visibility = ViewStates.Gone;
                        return;
                    }
                    else
                    {
                        var regservicio2 = new RegistrarServicioModel
                        {
                            id_Solicitud = servicio.id_Solicitud,
                            NumeroSolicitud = servicio.NumeroSolicitud,
                            codInstitucion = "Null",
                            codDesenlace = txtDestinoDesenlace.Text,
                           // codEstado = idestado,
                           // Estado = "C"
                        };

                        //var regservicio2 = servicio;

                        //regservicio2.HoraEstado = string.Format("{0:HH:mm}", System.DateTime.Now);
                        //regservicio2.codEstado = idestado;
                        //regservicio2.codInstitucion = "Null";
                        //regservicio2.codDesenlace = txtDestinoDesenlace.Text;
                        servicio.ID = ID;
                        servicio.Estado = servicio.Estado;
                        servicio.HoraEstado = servicio.HoraEstado;
                        servicio.codEstado = servicio.codEstado;
                        servicio.codInstitucion = regservicio2.codInstitucion;
                        servicio.codDesenlace = regservicio2.codDesenlace;
                        ServicioManager.SaveTask(servicio);
                        actualizarInstitucionDesenlace(regservicio2);
                     
                    }

                                    
                    Intent intent = new Intent();
                    intent.SetClass(BaseContext, typeof(Servicios));
                    intent.SetFlags(ActivityFlags.ReorderToFront);
                    StartActivity(intent);
                    Finish();
                    //
                    // Navigation.PopAsync();
                    // await Navigation.PopAsync();
                    break;
                case "Registrar Salida a Institución":
                    idestado = "005";
                    await GetIndexDato(txtDestinoDesenlace.Text);
                    if (string.IsNullOrEmpty(txtDestinoDesenlace.Text))
                    {
                        Toast.MakeText(this, "Debe ingresar una institución", ToastLength.Long).Show();
                       // mProgress.Visibility = ViewStates.Gone;
                        return;
                    }
                    else
                    {
                        var regservicio = new RegistrarServicioModel
                        {
                            id_Solicitud = servicio.id_Solicitud,
                            NumeroSolicitud = servicio.NumeroSolicitud,
                            codInstitucion = txtDestinoDesenlace.Text,
                            codDesenlace = "Null",
                            codEstado = idestado

                        };
                        //var regservicio = servicio;

                        //regservicio.HoraEstado = string.Format("{0:HH:mm}", System.DateTime.Now);
                        //regservicio.codEstado = idestado;
                        //regservicio.codInstitucion = txtDestinoDesenlace.Text ;
                        //regservicio.codDesenlace = "Null";
                        servicio.ID = ID;
                        servicio.Estado = servicio.Estado;
                        servicio.HoraEstado = servicio.HoraEstado;
                        servicio.codEstado = servicio.codEstado;
                        servicio.codInstitucion = regservicio.codInstitucion;
                        servicio.codDesenlace = regservicio.codDesenlace;
                        ServicioManager.SaveTask(servicio);
                        actualizarInstitucionDesenlace(regservicio);
                     
                    }

                    //regTransladoButton.BackgroundColor = Color.Purple;
                    btnRegistrarResultado.Text = "Registrar Llegada a Institución";
                    txtDestinoDesenlace.Enabled = false;
                    break;
                case "Registrar Llegada a Institución":
                    idestado = "006";
                    // regTransladoButton.BackgroundColor = Color.Teal;
                    btnRegistrarResultado.Text = "Registrar Salida de Institución";
                    break;
                case "Registrar Salida de Institución":
                    idestado = "007";
                    //regTransladoButton.BackgroundColor = Color.Green;
                    btnRegistrarResultado.Text = "Registrar Llegada a Base";
                    break;

                default:
                    idestado = "";
                    break;
            }

            if (!string.IsNullOrEmpty(idestado))
            {
                var regservicio = new RegistrarServicioModel
                {
                    id_Solicitud = servicio.id_Solicitud,
                    NumeroSolicitud = servicio.NumeroSolicitud,
                    HoraEstado = string.Format("{0:HH:mm}", System.DateTime.Now),
                    codEstado = idestado,
                    Estado = servicio.Estado

                };

                //actualizar localmente
                servicio.ID = ID;
                servicio.codEstado = regservicio.codEstado;
                servicio.HoraEstado = regservicio.HoraEstado;
                ServicioManager.SaveTask(servicio);

                GuardarDatos(regservicio);

               // mProgress.Visibility = ViewStates.Gone;

                Intent intent = new Intent();
                intent.SetClass(BaseContext, typeof(Servicios));
                intent.SetFlags(ActivityFlags.ReorderToFront);
                StartActivity(intent);
                Finish();
            }
        }


        #region "Metodos actualizacion BD" 
        //async Task
        private void GuardarDatos(RegistrarServicioModel regservicio)
        {

            //GetAddress();

            servicioDetalle = new ServicioItem();
            servicioDetalle.id_Solicitud = servicio.id_Solicitud;
            servicioDetalle.NumeroSolicitud = servicio.NumeroSolicitud;
            servicioDetalle.Nombre = servicio.nombrePaciente;
            servicioDetalle.Fecha = DateTime.Now;
            servicioDetalle.codMovil = movil;
            servicioDetalle.Estado = servicio.Estado;
            servicioDetalle.codEstado = servicio.codEstado;
            servicioDetalle.HoraEstado = servicio.HoraEstado;
            servicioDetalle.codInstitucion = regservicio.codInstitucion;
            servicioDetalle.codDesenlace = regservicio.codDesenlace;
            servicioDetalle.Enviado = false;
            servicioDetalle.AuditUsuario = usuario;
            servicioDetalle.AuditId = servicio.ID;
            servicioDetalle.GeoData = _locationText;
            servicioDetalle.Address = _addressText;
            ServicioItemManager.SaveTask(servicioDetalle);
            Toast.MakeText(this, "Registro guardado Correctamente", ToastLength.Long).Show();


            if (isBound)
            {
                RunOnUiThread(() =>
                {
                    binder.GetDemoService().SincronizarEstados();
                   // string text = binder.GetDemoService().GetText();
                    //Console.WriteLine("{0} returned from DemoService", text);
                  //  Toast.MakeText(this, string.Format("{0} returned from DemoService", text), ToastLength.Long).Show();
                }

            );
            }

        }


        private void actualizarInstitucionDesenlace(RegistrarServicioModel servTranslado)
        {
         

            servicioDetalle = new ServicioItem();
            servicioDetalle.id_Solicitud = servicio.id_Solicitud;
            servicioDetalle.NumeroSolicitud = servicio.NumeroSolicitud;
            servicioDetalle.Nombre = servicio.nombrePaciente;
            servicioDetalle.Fecha = DateTime.Now;
            servicioDetalle.codMovil = movil;
            servicioDetalle.Estado = servicio.Estado;
            servicioDetalle.codEstado = servicio.codEstado;
            servicioDetalle.HoraEstado = servicio.HoraEstado;
            servicioDetalle.codInstitucion = servTranslado.codInstitucion;
            servicioDetalle.codDesenlace = servTranslado.codDesenlace;
            servicioDetalle.Enviado = false;
            servicioDetalle.AuditUsuario = usuario;
            servicioDetalle.AuditId = servicio.ID;
            servicioDetalle.GeoData = _locationText;
            ServicioItemManager.SaveTask(servicioDetalle);
            Toast.MakeText(this, "Registro guardado Correctamtne", ToastLength.Long).Show();

            if (isBound)
            {
                RunOnUiThread(() =>
                {
                    binder.GetDemoService().SincronizarEstados();
                    //string text = binder.GetDemoService().GetText();
                    //Console.WriteLine("{0} returned from DemoService", text);
                   // Toast.MakeText(this, string.Format("{0} returned from DemoService", text), ToastLength.Long).Show();
                }

            );
            }
        }
        #endregion

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
                if (lblDestinoDesenlace.Text == "Desenlace")
                {
                    codtabla = "07";
                }
                else
                {
                    codtabla = "06";
                }
                //string url = string.Format("api/SasDatosApi?idtabla={0}&codigo={1}", codtabla, Id);
                //var response = await client.GetAsync(url);
                //result = response.Content.ReadAsStringAsync().Result;
                //Items = JsonConvert.DeserializeObject <List<Personas>> (result);

              //  busqueda = SasDatosManager.GetTask(1);

               busqueda= SasDatosManager.GetTaskTabCod(codtabla, Id);

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


        class DemoServiceConnection : Java.Lang.Object, IServiceConnection
        {
            RegistrarServicio activity;
            DemoServiceBinder binder;

            public DemoServiceBinder Binder
            {
                get
                {
                    return binder;
                }
            }

            public DemoServiceConnection(RegistrarServicio activity)
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


        #region"codigo comentado"
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private  void BtnRegistrarResultado_Click(object sender, EventArgs e)
        //{
        //    AlertDialog.Builder builder = new AlertDialog.Builder(this);
        //    builder.SetTitle("Confirmación");
        //    builder.SetMessage("Se encuetra a punto de actualizar el servicio. ¿Continuar?.");
        //    builder.SetCancelable(true);
        //    builder.SetPositiveButton("Si", delegate
        //    {

        //        mProgress.Indeterminate = true;
        //        mProgress.Visibility = ViewStates.Visible;
        //        GuardarResultados();

        //    });
        //    builder.SetNegativeButton("No", delegate { return; });
        //    builder.Show();



        //}
        //private async void GuardarResultados()
        //{
        //    string idestado = "";

        //    switch (btnRegistrarResultado.Text)
        //    {
        //        case "Registrar Salida a Base":
        //            idestado = "005";
        //            //volverBaseButton.BackgroundColor = Color.Green;
        //            // btnRegistrarResultado.Text = "Servicio Finalizado";
        //            btnRegistrarResultado.Text = "Registrar Llegada a Base";
        //            break;

        //        //case "Servicio Finalizado":
        //        //    idestado = "009";
        //        //    //volverBaseButton.BackgroundColor = Color.Purple;
        //        //    btnRegistrarResultado.Text = "Registrar Llegada a Base";

        //        //    break;
        //        case "Registrar Llegada a Base":
        //            idestado = "008";
        //            // volverBaseButton.BackgroundColor = Color.Maroon;
        //            btnRegistrarResultado.Text = "Registrar Desenlace";

        //            txtDestinoDesenlace.Visibility = ViewStates.Visible;
        //            lblDestinoDesenlace.Visibility = ViewStates.Visible;
        //            txtDestinoDesenlace.Visibility = ViewStates.Visible;
        //            btnBuscar.Visibility = ViewStates.Visible;
        //            txtDestinoDesenlace.Enabled = true;

        //            lblDestinoDesenlace.Text = "Desenlace";

        //            txtDestinoDesenlace.Text = string.Empty;
        //            lblDescrpcionDestinoDesenlace.Text = string.Empty;

        //            //volverBaseButton.IsEnabled = false;
        //            break;
        //        case "Registrar Desenlace":
        //            await GetIndexDato(txtDestinoDesenlace.Text);
        //            idestado = "009";
        //            if (string.IsNullOrEmpty(txtDestinoDesenlace.Text))
        //            {


        //                Toast.MakeText(this, "Debe ingresar un desenlace", ToastLength.Long).Show();
        //                return;
        //            }
        //            else
        //            {
        //                var regservicio2 = new RegistrarServicioModel
        //                {
        //                    id_Solicitud = servicio.id_Solicitud,
        //                    NumeroSolicitud = servicio.NumeroSolicitud,
        //                    codInstitucion = "Null",
        //                    codDesenlace = txtDestinoDesenlace.Text
        //                };

        //                //var regservicio2 = servicio;

        //                //regservicio2.HoraEstado = string.Format("{0:HH:mm}", System.DateTime.Now);
        //                //regservicio2.codEstado = idestado;
        //                //regservicio2.codInstitucion = "Null";
        //                //regservicio2.codDesenlace = txtDestinoDesenlace.Text;

        //                await actualizarInstitucionDesenlace(regservicio2);
        //            }

        //            Intent intent = new Intent();

        //            intent.SetClass(BaseContext, typeof(Servicios));

        //            intent.SetFlags(ActivityFlags.ReorderToFront);

        //            StartActivity(intent);

        //            Finish();

        //            //
        //            // Navigation.PopAsync();
        //            // await Navigation.PopAsync();
        //            break;
        //        case "Registrar Salida a Institución":
        //            idestado = "005";
        //            await GetIndexDato(txtDestinoDesenlace.Text);
        //            if (string.IsNullOrEmpty(txtDestinoDesenlace.Text))
        //            {

        //                Toast.MakeText(this, "Debe ingresar una institución", ToastLength.Long).Show();

        //                return;
        //            }
        //            else
        //            {
        //                var regservicio = new RegistrarServicioModel
        //                {
        //                    id_Solicitud = servicio.id_Solicitud,
        //                    NumeroSolicitud = servicio.NumeroSolicitud,
        //                    codInstitucion = txtDestinoDesenlace.Text,
        //                    codDesenlace = "Null"

        //                };
        //                //var regservicio = servicio;

        //                //regservicio.HoraEstado = string.Format("{0:HH:mm}", System.DateTime.Now);
        //                //regservicio.codEstado = idestado;
        //                //regservicio.codInstitucion = txtDestinoDesenlace.Text ;
        //                //regservicio.codDesenlace = "Null";

        //                await actualizarInstitucionDesenlace(regservicio);
        //            }

        //            //regTransladoButton.BackgroundColor = Color.Purple;
        //            btnRegistrarResultado.Text = "Registrar Llegada a Institución";
        //            txtDestinoDesenlace.Enabled = false;
        //            break;
        //        case "Registrar Llegada a Institución":
        //            idestado = "006";
        //            // regTransladoButton.BackgroundColor = Color.Teal;
        //            btnRegistrarResultado.Text = "Registrar Salida de Institución";
        //            break;
        //        case "Registrar Salida de Institución":
        //            idestado = "007";
        //            //regTransladoButton.BackgroundColor = Color.Green;
        //            btnRegistrarResultado.Text = "Registrar Llegada a Base";
        //            break;

        //        default:
        //            idestado = "";
        //            break;
        //    }

        //    if (!string.IsNullOrEmpty(idestado))
        //    {
        //        var regservicio = new RegistrarServicioModel
        //        {
        //            id_Solicitud = servicio.id_Solicitud,
        //            NumeroSolicitud = servicio.NumeroSolicitud,
        //            HoraEstado = string.Format("{0:HH:mm}", System.DateTime.Now),
        //            codEstado = idestado

        //        };
        //        //var regservicio = servicio;

        //        //regservicio.HoraEstado = string.Format("{0:HH:mm}", System.DateTime.Now);
        //        //regservicio.codEstado = idestado;
        //        //regservicio.HoraEstado = string.Format("{0:HH:mm}", System.DateTime.Now);

        //        await GuardarDatos(regservicio);

        //        mProgress.Visibility = ViewStates.Gone;
        //    }
        //}
        #endregion
    }
}