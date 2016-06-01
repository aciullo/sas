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
        private ServicioLocal servicio;
        private ServicioItem servicioDetalle;
        private int ID = 0;
        private List<SasDatosModel> sasdatos;
        private SasDatosModel sasdatosBusqueda;
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
        Button btnRegistroInicial;
        Button btnVolverBase;
        Button btnTranslado;
        Button btnRegistrarResultado;
        Button btnBuscar;
        Button btnRegistroLocal;
        TextView lblDestinoDesenlace;
        TextView lblDescrpcionDestinoDesenlace;
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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            //asignar el diseño
            SetContentView(Resource.Layout.RegistrarServiciolayout);

            //recuperar IP con WEBAPI
            session = new UserSessionManager(this);
            IPCONN = session.getAccessConn();
            usuario = session.getAccessKey();
            movil = session.getAccessIdmovil();

            //referenciar al manager d e localizaciones
            InitializeLocationManager();

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

            //recibir datos de la actividad predecesora
            // servicio = this.Intent.GetParcelableExtra("ServiciosDet") as ServiciosModel;
            // ID = this.Intent.GetStringExtra("ServiciosDet");
            // servicio = ServicioManager.GetTask(Convert.ToInt32(ID));
            ID = Intent.Extras.GetInt("ServiciosDet");

            servicio = ServicioManager.GetTask(ID);

            // sasdatosBusqueda = this.Intent.GetParcelableExtra("sasDatos") as SasDatosModel;



            //mostrar los datos recibidos
            txtNroSolicitud.Text = servicio.NumeroSolicitud.ToString();
            txtNombrePaciente.Text = servicio.nombrePaciente;
            txtEdad.Text = servicio.edadPaciente.ToString();


            //mostrar de la busqueda
            //if (sasdatosBusqueda != null)
            //{
            //    txtDestinoDesenlace.Text = sasdatosBusqueda.codigo;
            //    GetIndexDato(txtDestinoDesenlace.Text);

            //}

            //variables
            codEstadoRecibido =  servicio.codEstado;
            codInstitucionRecibido = servicio.codInstitucion;
            codDesenlace = servicio.codDesenlace;

            //mostrar botones segun ÚLTIMO estado
            LoadStateButtons(codEstadoRecibido);

            //asignar los eventos a los controles
            btnRegistroInicial.Click += BtnRegistroInicial_Click;
            btnVolverBase.Click += BtnVolverBase_Click;
           // btnRegistrarResultado.Click += BtnRegistrarResultado_Click;
            btnTranslado.Click += BtnTranslado_Click;
            btnRegistroLocal.Click += BtnRegistroLocal_Click;
            //txtDestinoDesenlace.KeyPress += async (object sender, View.KeyEventArgs e) =>
            //{
            //    e.Handled = false;
            //    if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            //    {
            //        //Toast.MakeText(this, txtDestinoDesenlace.Text, ToastLength.Short).Show();
            //        await GetIndexDato(txtDestinoDesenlace.Text);
            //        e.Handled = true;
            //    }
            //};

            txtDestinoDesenlace.KeyPress += TxtDestinoDesenlace_KeyPress;
            txtDestinoDesenlace.FocusChange += TxtDestinoDesenlace_FocusChange;
            btnBuscar.Click += BtnBuscar_Click;
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


        protected override void OnResume()
        {
            base.OnResume();
            _locationManager = GetSystemService(Context.LocationService) as LocationManager;
            if (_locationManager.IsProviderEnabled(LocationManager.GpsProvider))
            {
                _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
                Log.Debug(TAG, "Listening for location updates using " + _locationProvider + ".");
            }
            else
            {

                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Location Services Not Active");
                builder.SetMessage("Please enable Location Services and GPS");
                builder.SetPositiveButton("OK", delegate {
                    // Show location settings when the user acknowledges the alert dialog
                    Intent intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                    StartActivity(intent);
                    
                });

                Dialog alertDialog = builder.Create();
                alertDialog.SetCanceledOnTouchOutside(false);
                alertDialog.Show();
              
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
            Log.Debug(TAG, "No longer listening for location updates.");
        }

        #region"GPS"
      
        async void GetAddress()
        {
            if (_currentLocation == null)
            {
                _addressText = "Can't determine the current address. Try again in a few minutes.";
                return;
            }

            Address address = await ReverseGeocodeCurrentLocation();
            DisplayAddress(address);
        }

        async Task<Address> ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList =
                await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);

            Address address = addressList.FirstOrDefault();
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


        private void BtnRegistroLocal_Click(object sender, EventArgs e)
        {
            var newActivity = new Intent(this, typeof(RegistrarServicioLocal));
            StartActivity(newActivity);
        }

        protected override async void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                sasdatosBusqueda = data.GetParcelableExtra("sasDatos") as SasDatosModel;


                if (sasdatosBusqueda != null)
                {
                    txtDestinoDesenlace.Text = sasdatosBusqueda.codigo;
                    await GetIndexDato(txtDestinoDesenlace.Text);

                }
            }

        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            if (newConfig.Orientation == Android.Content.Res.Orientation.Portrait)
            {
                Toast.MakeText(this, "Changed to portrait", ToastLength.Long).Show();

               
            }
            else if (newConfig.Orientation == Android.Content.Res.Orientation.Landscape)
            {
                Toast.MakeText(this, "Changed to landscape", ToastLength.Long).Show();
              
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
                case ("001"):
                    btnRegistroInicial.Text = "Registrar Salida de Base";
                    break;
                case ("002"):
                    btnRegistroInicial.Text = "Registrar Salida de Base";
                    break;
                case "003":
                    btnRegistroInicial.Text = "Registrar Llegada a Servicio";
                    break;
                case "004":
                    btnRegistroInicial.Enabled = false;
                    btnRegistroInicial.Visibility = ViewStates.Invisible;
                    btnVolverBase.Enabled = true;
                    btnTranslado.Enabled = true;

                    break;
                case "005":
                   // btnRegistroInicial.Enabled = false;
                    btnVolverBase.Enabled = false;
                    btnTranslado.Enabled = false;
                    btnRegistroInicial.Visibility = ViewStates.Visible;
                    btnVolverBase.Visibility = ViewStates.Invisible;
                    btnTranslado.Visibility = ViewStates.Invisible;

                    btnRegistrarResultado.Visibility = ViewStates.Invisible;
                    if (string.IsNullOrEmpty(codInstitucionRecibido)&& (string.IsNullOrEmpty(codDesenlace)))
                    {
                        txtDestinoDesenlace.Visibility = ViewStates.Invisible;
                        lblDestinoDesenlace.Visibility = ViewStates.Invisible;
                        txtDestinoDesenlace.Visibility = ViewStates.Invisible;
                        btnBuscar.Visibility= ViewStates.Invisible;
                        btnRegistroInicial.Text = "Registrar Llegada a Base";
                    }
                    else
                    {
                        txtDestinoDesenlace.Visibility = ViewStates.Visible;
                        lblDestinoDesenlace.Visibility = ViewStates.Visible;
                        txtDestinoDesenlace.Visibility = ViewStates.Visible;
                        btnBuscar.Visibility = ViewStates.Visible;
                        //mostrar institucion
                        txtDestinoDesenlace.Text = codInstitucionRecibido;
                        await GetIndexDato(txtDestinoDesenlace.Text);
                        btnRegistroInicial.Text = "Registrar Llegada a Institución";
                        txtDestinoDesenlace.Enabled = false;
                    }
                    break;
                case "006":
                    btnRegistroInicial.Enabled = true;
                    btnVolverBase.Enabled = false;
                    btnTranslado.Enabled = false;
                    btnRegistroInicial.Visibility = ViewStates.Visible;
                    btnVolverBase.Visibility = ViewStates.Invisible;
                    btnTranslado.Visibility = ViewStates.Invisible;

                    btnRegistrarResultado.Visibility = ViewStates.Invisible;

                    txtDestinoDesenlace.Visibility = ViewStates.Visible;
                    lblDestinoDesenlace.Visibility = ViewStates.Visible;
                    txtDestinoDesenlace.Visibility = ViewStates.Visible;
                    btnBuscar.Visibility = ViewStates.Visible;

                    lblDestinoDesenlace.Text = "Institución";
                    txtDestinoDesenlace.Text = codInstitucionRecibido;
                    await GetIndexDato(txtDestinoDesenlace.Text);
                    txtDestinoDesenlace.Enabled = false;
                    btnRegistroInicial.Text = "Registrar Salida de Institución";
                    break;
                case "007":
                    btnRegistroInicial.Enabled = true;
                    btnVolverBase.Enabled = false;
                    btnTranslado.Enabled = false;

                    btnRegistroInicial.Visibility = ViewStates.Visible;
                    btnVolverBase.Visibility = ViewStates.Invisible;
                    btnTranslado.Visibility = ViewStates.Invisible;

                    btnRegistrarResultado.Visibility = ViewStates.Invisible;

                    btnRegistroInicial.Text = "Registrar Llegada a Base";

                    break;
                case "008":
                    btnRegistroInicial.Enabled = true;
                    btnVolverBase.Enabled = false;
                    btnTranslado.Enabled = false;

                    btnRegistroInicial.Visibility = ViewStates.Visible;

                    btnVolverBase.Visibility = ViewStates.Invisible;
                    btnTranslado.Visibility = ViewStates.Invisible;
                    btnRegistrarResultado.Visibility = ViewStates.Invisible;

                    btnRegistroInicial.Text = "Registrar Desenlace";
                    txtDestinoDesenlace.Visibility = ViewStates.Visible;
                    lblDestinoDesenlace.Visibility = ViewStates.Visible;
                    txtDestinoDesenlace.Visibility = ViewStates.Visible;
                    btnBuscar.Visibility = ViewStates.Visible;
                    txtDestinoDesenlace.Enabled = true;

                    lblDestinoDesenlace.Text = "Desenlace";

                    txtDestinoDesenlace.Text = string.Empty;
                    lblDescrpcionDestinoDesenlace.Text = string.Empty;
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
            btnVolverBase.Enabled = false;
            btnTranslado.Enabled = false;
            txtDestinoDesenlace.Enabled = true;

            btnRegistroInicial.Text = "Registrar Salida a Institución";
            btnRegistroInicial.Visibility = ViewStates.Visible;
            btnVolverBase.Visibility = ViewStates.Invisible;
            btnTranslado.Visibility = ViewStates.Invisible;
            btnRegistrarResultado.Visibility = ViewStates.Invisible;
            btnBuscar.Visibility = ViewStates.Visible;
            txtDestinoDesenlace.Visibility = ViewStates.Visible;
            txtDestinoDesenlace.Visibility = ViewStates.Visible;
            lblDestinoDesenlace.Visibility = ViewStates.Visible;

            lblDestinoDesenlace.Text = "Institución";
            txtDestinoDesenlace.Text = string.Empty;
            lblDescrpcionDestinoDesenlace.Text = string.Empty;
        }

        private void BtnVolverBase_Click(object sender, EventArgs e)
        {
            btnRegistroInicial.Text = "Registrar Salida a Base";
            btnRegistrarResultado.Visibility= ViewStates.Invisible;
            btnVolverBase.Enabled = false;
            btnTranslado.Enabled = false;
           // btnRegistroInicial.Visibility = ViewStates.Invisible;
            btnVolverBase.Visibility = ViewStates.Invisible;
            btnTranslado.Visibility = ViewStates.Invisible;
        }

        private void BtnRegistroInicial_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Confirmación");
            builder.SetMessage("Se encuetra a punto de actualizar el servicio. ¿Continuar?.");
            builder.SetCancelable(true);
            builder.SetPositiveButton("Si", delegate
            {
                mProgress.Indeterminate = true;
                mProgress.Visibility = ViewStates.Visible;
                GuardarEstadoInicial();
             });
            builder.SetNegativeButton("No", delegate { return; });
            builder.Show();
         
        }

        private async void GuardarEstadoInicial()
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
                    btnRegistrarResultado.Text = "Registrar Desenlace";
                    txtDestinoDesenlace.Visibility = ViewStates.Visible;
                    lblDestinoDesenlace.Visibility = ViewStates.Visible;
                    txtDestinoDesenlace.Visibility = ViewStates.Visible;
                    btnBuscar.Visibility = ViewStates.Visible;
                    txtDestinoDesenlace.Enabled = true;
                    lblDestinoDesenlace.Text = "Desenlace";
                    txtDestinoDesenlace.Text = string.Empty;
                    lblDescrpcionDestinoDesenlace.Text = string.Empty;
                    //volverBaseButton.IsEnabled = false;
                    break;
                case "Registrar Desenlace":
                    await GetIndexDato(txtDestinoDesenlace.Text);
                    idestado = "009";
                    if (string.IsNullOrEmpty(txtDestinoDesenlace.Text))
                    {
                        Toast.MakeText(this, "Debe ingresar un desenlace", ToastLength.Long).Show();
                        return;
                    }
                    else
                    {
                        var regservicio2 = new RegistrarServicioModel
                        {
                            id_Solicitud = servicio.id_Solicitud,
                            NumeroSolicitud = servicio.NumeroSolicitud,
                            codInstitucion = "Null",
                            codDesenlace = txtDestinoDesenlace.Text
                        };
                        //var regservicio2 = servicio;
                        //regservicio2.HoraEstado = string.Format("{0:HH:mm}", System.DateTime.Now);
                        //regservicio2.codEstado = idestado;
                        //regservicio2.codInstitucion = "Null";
                        //regservicio2.codDesenlace = txtDestinoDesenlace.Text;

                        //actualizar localmente

                        servicio.Estado = regservicio2.Estado;
                        servicio.HoraEstado = regservicio2.HoraEstado;
                        servicio.codEstado = regservicio2.codEstado;
                        servicio.codInstitucion = regservicio2.codInstitucion;
                        servicio.codDesenlace = regservicio2.codDesenlace;
                        ServicioManager.SaveTask(servicio);


                        await actualizarInstitucionDesenlace(regservicio2);
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
                        return;
                    }
                    else
                    {
                        var regservicio = new RegistrarServicioModel
                        {
                            id_Solicitud = servicio.id_Solicitud,
                            NumeroSolicitud = servicio.NumeroSolicitud,
                            codInstitucion = txtDestinoDesenlace.Text,
                            codDesenlace = "Null"
                        };
                        //var regservicio = servicio;
                        //regservicio.HoraEstado = string.Format("{0:HH:mm}", System.DateTime.Now);
                        //regservicio.codEstado = idestado;
                        //regservicio.codInstitucion = txtDestinoDesenlace.Text ;
                        //regservicio.codDesenlace = "Null";

                        servicio.Estado = regservicio.Estado;
                        servicio.HoraEstado = regservicio.HoraEstado;
                        servicio.codEstado = regservicio.codEstado;
                        servicio.codInstitucion = regservicio.codInstitucion;
                        servicio.codDesenlace = regservicio.codDesenlace;
                        ServicioManager.SaveTask(servicio);

                        await actualizarInstitucionDesenlace(regservicio);
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
                    Estado= servicio.Estado
                    
                };
                //var regservicio = servicio;
                //regservicio.HoraEstado = string.Format("{0:HH:mm}", System.DateTime.Now);
                //regservicio.codEstado = idestado;
                //regservicio.HoraEstado = string.Format("{0:HH:mm}", System.DateTime.Now);

                //actualizar localmente
                servicio.Estado = regservicio.Estado;
                servicio.codEstado = regservicio.codEstado;
                servicio.HoraEstado = regservicio.HoraEstado;
                servicio.codInstitucion = regservicio.codInstitucion;
                servicio.codDesenlace = regservicio.codDesenlace;
                ServicioManager.SaveTask(servicio);

                await GuardarDatos(regservicio);
                
              

                mProgress.Visibility = ViewStates.Gone;

            }
        }
        #region "Metodos actualizacion BD"
        private async Task GuardarDatos(RegistrarServicioModel regservicio)
        {
            string result;
            try
            {
                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;
                client.BaseAddress = new Uri(IPCONN);
                //var uri = new Uri (string.Format ("http://181.120.121.221:88/api/sas_ServiciosApi/{0}/{1}/{2}", deviceUser.codMovil,"001","P" ));
                string url = string.Format("/api/UpdServiciosApi?idsolicitud={0}&codestado={1}&hora={2}", regservicio.id_Solicitud, regservicio.codEstado, regservicio.HoraEstado);
                var response = await client.GetAsync(url);
                result = response.Content.ReadAsStringAsync().Result;
                //Items = JsonConvert.DeserializeObject <List<Personas>> (result);
                if (result.Contains("Error"))
                {
                    Toast.MakeText(this, "Error", ToastLength.Long).Show();
                    //using (var datos = new DAServicioDet())
                    //{
                    //    datos.InsertServicio(regservicio);
                    //}
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();

                //using (var datos = new DAServicioDet())
                //{
                //    datos.InsertServicio(regservicio);
                //}

                GetAddress();

                servicioDetalle = new ServicioItem();
                servicioDetalle.id_Solicitud = servicio.id_Solicitud;
                servicioDetalle.NumeroSolicitud = servicio.NumeroSolicitud;
                servicioDetalle.Nombre = servicio.nombrePaciente;
                servicioDetalle.Fecha = DateTime.Now;
                servicioDetalle.codMovil = movil;
                servicioDetalle.Estado= regservicio.Estado;
                servicioDetalle.codEstado = regservicio.codEstado;
                servicioDetalle.HoraEstado = regservicio.HoraEstado;
                servicioDetalle.codInstitucion = regservicio.codInstitucion;
                servicioDetalle.codDesenlace = regservicio.codDesenlace;
                servicioDetalle.Enviado = false;
                servicioDetalle.AuditUsuario = usuario;
                servicioDetalle.AuditId = servicioDetalle.ID;
                servicioDetalle.GeoData = _locationText;
                ServicioItemManager.SaveTask(servicioDetalle);



                return;
            }
            //waitActivityIndicator.IsRunning = false;
            Toast.MakeText(this, "Registro guardado Correctamtne", ToastLength.Long).Show();
        }


        private async Task actualizarInstitucionDesenlace(RegistrarServicioModel servTranslado)
        {

           
            string result;
            var jsonResquest = JsonConvert.SerializeObject(servTranslado);
            var content = new StringContent(jsonResquest, Encoding.UTF8, "text/json");
            try
            {
                //transladoButton.IsEnabled = false;
                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;
                //	client.BaseAddress = new Uri ("http://181.120.121.221:88");
                System.Net.Http.HttpResponseMessage response;
                client.BaseAddress = new Uri(IPCONN);
                if (servTranslado.codDesenlace == "Null")
                {
                    var url = string.Format("/api/ABMServiciosApi?idsolicitud={0}&nrosolicitud={1}&destino={2}", servTranslado.id_Solicitud, servTranslado.NumeroSolicitud, servTranslado.codInstitucion);
                    response = await client.GetAsync(url);
                }
                else
                {
                    var url = (string.Format("/api/ABMServiciosApi?idsolicitud={0}&nrosolicitud={1}&destino={2}&desenlace={3}", servTranslado.id_Solicitud, servTranslado.NumeroSolicitud, servTranslado.codInstitucion, servTranslado.codDesenlace));
                    response = await client.GetAsync(url);
                }
                //string url = string.Format ("/api/ABMServiciosApi?idsolicitud={0}&nrosolicitud={1}&destino={2}&IdProductoFinal={3}", servTranslado.id_Solicitud,servTranslado.NumeroSolicitud,servTranslado.codServicioFinal, servTranslado.codProductoFinal);
                //?idsolicitud={idsolicitud}&nrosolicitud={nrosolicitud}&destino={destino}&IdProductoFinal={IdProductoFinal}
                //var response= await client.GetAsync(uri);
                result = response.Content.ReadAsStringAsync().Result;
                //Items = JsonConvert.DeserializeObject <List<Personas>> (result);
                // transladoButton.IsEnabled = true;
                //guaardar localmente
                if (result.Contains("Error"))
                {
                    Toast.MakeText(this, "Error", ToastLength.Long).Show();

                    using (var datos = new DAServicioDet())
                    {
                        datos.InsertServicio(servTranslado);
                    }
                }
            }
            catch (Exception ex)
            {
                //guaardar localmente
                Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();
                // transladoButton.IsEnabled = true;
                // waitActivityIndicator.IsRunning = false;
                //using (var datos = new DAServicioDet())
                //{
                //    datos.InsertServicio(servTranslado);
                //}


                GetAddress();

                servicioDetalle = new ServicioItem();
                servicioDetalle.id_Solicitud = servicio.id_Solicitud;
                servicioDetalle.NumeroSolicitud = servicio.NumeroSolicitud;
                servicioDetalle.Nombre = servicio.nombrePaciente;
                servicioDetalle.Fecha = DateTime.Now;
                servicioDetalle.codMovil = movil;
                servicioDetalle.Estado = servTranslado.Estado;
                servicioDetalle.codEstado = servTranslado.codEstado;
                servicioDetalle.HoraEstado = servTranslado.HoraEstado;
                servicioDetalle.codInstitucion = servTranslado.codInstitucion;
                servicioDetalle.codDesenlace = servTranslado.codDesenlace;
                servicioDetalle.Enviado = false;
                servicioDetalle.AuditUsuario = usuario;
                servicioDetalle.AuditId = servicioDetalle.ID;
                servicioDetalle.GeoData = _locationText;
                ServicioItemManager.SaveTask(servicioDetalle);


                return;
            }

            Toast.MakeText(this, "Registro guardado Correctamtne", ToastLength.Long).Show();
            // waitActivityIndicator.IsRunning = false;
            //await Navigation.PushAsync (new PersonasPage(persona));

        }
        #endregion

        async Task GetIndexDato(string Id)
        {
            string result;
            try
            {
                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;
                client.BaseAddress = new Uri(IPCONN);
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
                string url = string.Format("/api/SasDatosApi?idtabla={0}&codigo={1}", codtabla, Id);
                var response = await client.GetAsync(url);
                result = response.Content.ReadAsStringAsync().Result;
                //Items = JsonConvert.DeserializeObject <List<Personas>> (result);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();
                return;
            }
            sasdatos = JsonConvert.DeserializeObject<List<SasDatosModel>>(result);
            if (sasdatos.Count > 0)
            {
                lblDescrpcionDestinoDesenlace.Text = string.Format("{0}", sasdatos[0].descripcion);
            }
            else
            {
                txtDestinoDesenlace.Text = string.Empty;
                lblDescrpcionDestinoDesenlace.Text = string.Empty;
               // institucionEntry.Focus();
                //await DisplayAlert("Error", "Código no existe", "Aceptar");
                Toast.MakeText(this, "Código no existe", ToastLength.Long).Show();
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