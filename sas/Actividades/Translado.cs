using System;
using System.Collections.Generic;
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
        EditText txtSAT;
        EditText txtTemperatura;
        EditText txtglasgow;
        EditText txtGlicemia;
        EditText txtMedico;
        EditText txtDestinoDesenlace;

        Button btnGuardarTranslado;
        Button btnBuscar;
    
        
        TextView lblDescrpcionDestinoDesenlace;
        
        ProgressBar mProgress;

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
            txtTA = FindViewById<EditText>(Resource.Id.txtTA);
            txtFC = FindViewById<EditText>(Resource.Id.txtFC);
            txtSAT = FindViewById<EditText>(Resource.Id.txtSAT);
            txtTemperatura = FindViewById<EditText>(Resource.Id.txtTemperatura);
            txtglasgow = FindViewById<EditText>(Resource.Id.txtglasgow);
            txtGlicemia = FindViewById<EditText>(Resource.Id.txtGlicemia);
            txtMedico = FindViewById<EditText>(Resource.Id.txtMedico);
            txtDestinoDesenlace = FindViewById<EditText>(Resource.Id.txtDestinoDesenlace);
            btnGuardarTranslado = FindViewById<Button>(Resource.Id.btnGuardarTranslado);
            btnBuscar = FindViewById<Button>(Resource.Id.btnBuscar);
            mProgress = FindViewById<ProgressBar>(Resource.Id.mProgress);
            mProgress.Visibility = ViewStates.Invisible;

            //asignar eventos
            
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


        #region "Menu"
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.busqueda, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            //Toast.MakeText(this, "Top ActionBar pressed: " + item.TitleFormatted, ToastLength.Short).Show();
          
            if (item.TitleFormatted.ToString() == "Buscar Institución")
            {
                string codtabla = "";
                var newActivity = new Intent(this, typeof(Buscar));
                //newActivity.PutExtra("ServiciosDet", servicio);
                codtabla = "06";
                newActivity.PutExtra("codtabla", codtabla);
                StartActivityForResult(newActivity, 0);

            }

           if (item.TitleFormatted.ToString() == "Buscar Médico")
            {
          

            }
            return base.OnOptionsItemSelected(item);
        }
        #endregion


    }
}