using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using sas.Clases;
using Android.Util;
using System.Text;

using System.Net.Http.Headers;
using Android.Gms.Common;

namespace sas
{
    [Activity(Label = "sas", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/MyCustomTheme")]
    public class MainActivity : Activity
    {
        EditText txtUsuario;
        EditText txtClave;
        Button btnIngresar;

        private ProgressBar mProgress;
        
        private Handler mHandler = new Handler();

        // Session Manager Class
        UserSessionManager session;
        string IPCONN = "";

        private clsAutenticacion auth;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

           // ActionBar.Title = "Ingreso al sistema";
           
            // Set our view from the "main" layout resource

            SetContentView(Resource.Layout.LoginLayout);

            // Get our button from the layout resource,
            // and attach an event to it
            //

            auth = new clsAutenticacion();

            // Session Manager
            session = new UserSessionManager(this);
           

            txtUsuario = FindViewById<EditText>(Resource.Id.txtUsuario);
            txtClave = FindViewById<EditText>(Resource.Id.txtClave);
            btnIngresar = FindViewById<Button>(Resource.Id.btnIngresar);
            mProgress = FindViewById<ProgressBar>(Resource.Id.mProgress);

            //button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };

            btnIngresar.Click += BtnIngresar_Click;
            mProgress.Visibility = ViewStates.Invisible;

          //recuperar la base para la coneccion
            IPCONN = session.getAccessConn();


            // Start lengthy operation in a background thread
            //mProgress.Max = 100;
            //mProgress.Progress = 0;
            //while (mProgressStatus < 100)
            //{
            //    mProgressStatus = mProgressStatus + 1;

            //    mProgress.IncrementProgressBy(mProgressStatus);
            //};



            if (session.isLoggedIn())
            {
                //StartService(new Intent("com.sas.searchpending"));
                StartService(new Intent("com.xamarin.sas"));
                Intent newActivity = new Intent(this, typeof(Servicios));
                    Bundle valuesForActivity = new Bundle();
                    valuesForActivity.PutInt("GPS", 0);
                    newActivity.PutExtras(valuesForActivity);
                    StartActivity(newActivity);


                if (IsPlayServicesAvailable())
                {
                    var intent = new Intent(this, typeof(RegistrationIntentService));
                    StartService(intent);
                }

                Finish();
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
                    //Finish();
                }
                Log.Debug("gcm", msgText);
                return false;
            }
            else
            {
                msgText = "Google Play Services is available.";
                //builder.SetMessage(msgText);
                //builder.Show();
                Log.Debug("gcm", msgText);

                return true;
            }
        }
        private async void BtnIngresar_Click(object sender, EventArgs e)
        {

            btnIngresar.Enabled = false;
            mProgress.Indeterminate = true;
            mProgress.Visibility = ViewStates.Visible;
            await myMethod();

            mProgress.Visibility = ViewStates.Gone;

      
           

        }

        private async Task myMethod()
        {

            if (txtUsuario.Text == string.Empty)
            {
                Toast.MakeText(this, "Ingrese su usuario", ToastLength.Long).Show();
                return;
            }

            if (txtClave.Text == string.Empty)

            {
                Toast.MakeText(this, "Ingrese su clave", ToastLength.Long).Show();

                return;

            }


            //var result;

          
            try
            {
                
                Toast.MakeText(this, "Procesando petición de ingreso", ToastLength.Long).Show();

                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;

                //client.BaseAddress = new Uri ("http://181.120.121.221:88");
                client.BaseAddress = new Uri(IPCONN);

                var form = new Dictionary<string, string>
               {
                   {"grant_type", "password"},
                   {"username", txtUsuario.Text},
                   {"password", auth.Encripta(txtClave.Text)},
               };

               


                // var uri = new Uri(string.Format("http://181.120.121.221:88/api/DeviceUsersApi/{0}/{1}", txtUsuario.Text, txtClave.Text));

                // string url = string.Format ("/api/DeviceUsersApi/{0}/{1}", txtUsuario.Text, txtClave.Text);

                string url = string.Format("/token");


               // var response = await client.PostAsync(url, new FormUrlEncodedContent(form)).Result);

                var tokenResponse = client.PostAsync(client.BaseAddress + "/token", new FormUrlEncodedContent(form)).Result;
                var result = tokenResponse.Content.ReadAsStringAsync();
                //Items = JsonConvert.DeserializeObject <List<Personas>> (result);


                var token = new Dictionary<string, string>
                  {
                   {"access_token", ""},
                   {"token_type", ""},
                   {"expires_in", ""},
                  }; 
                 token=   JsonConvert.DeserializeObject<Dictionary<string, string>>(result.Result);

                if (string.IsNullOrEmpty(result.Result) || result.Result == "null" || result.Result.Contains("error"))
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Aviso");
                    builder.SetMessage("Usuario o clave no válidos.");
                    builder.SetCancelable(false);
                    builder.SetPositiveButton("OK", delegate { return; });
                    builder.Show();
                    //Toast.MakeText(this, "Usuario o clave no validos", ToastLength.Long).Show();
                    btnIngresar.Enabled = true;
                    return;
                }
                string access_token="";
                token.TryGetValue("access_token", out access_token);
                //  Toast.MakeText(this, "Ingresando...", ToastLength.Short).Show();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);

                url = string.Format("/api/UsersApi/{0}/{1}", txtUsuario.Text, auth.Encripta(txtClave.Text)); ;


                var response = await client.GetAsync(url);

                result = response.Content.ReadAsStringAsync();

                var deviceUser = JsonConvert.DeserializeObject<List<DeviceUserModel>>(result.Result);

             

                session.createLoginSession((deviceUser[0].nombres + " " + deviceUser[0].apellidos), deviceUser[0].codMovil, access_token, deviceUser[0].usuario);

               // StartService(new Intent("com.sas.searchpending"));
                StartService(new Intent("com.xamarin.sas"));
                //Bundle valuesForActivity = new Bundle();
                //valuesForActivity.PutString("user", result.Result);
                // valuesForActivity.PutStringArrayList("user", deviceUser);
                Intent newActivity = new Intent(this, typeof(Servicios));

                //newActivity.PutExtras(valuesForActivity);
                Bundle valuesForActivity = new Bundle();
                valuesForActivity.PutInt("GPS", 0);
                newActivity.PutExtras(valuesForActivity);
                StartActivity(newActivity);
               // Toast.MakeText(this, "OK", ToastLength.Long).Show();
                btnIngresar.Enabled = true;
                if (IsPlayServicesAvailable())
                {
                    var intent = new Intent(this, typeof(RegistrationIntentService));
                    StartService(intent);
                }
                Finish();

            }
            catch (Exception ex)
            {

                //Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();

                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Aviso");
                builder.SetMessage("No hay conexión intente más tarde");
                builder.SetCancelable(true);
                builder.SetPositiveButton("OK", delegate { return; });
                builder.Show();

                btnIngresar.Enabled = true;

                Log.Debug("Error en login", ex.Message);

                return;
            }
        }

        #region "codigo comentado"
        //actualizar idregistro
        //string result;

        //var person = new SimpledeviceUser();
        //person.usuario = "01";
        //person.pass = "1234";
        //person.codMovil = "10";
        //person.nombres = "Pepe";
        //person.apellidos = "Gonzalez";
        //person.idRegistro="prueba2";



        //var jsonResquest = JsonConvert.SerializeObject(person);
        //var content = new StringContent(jsonResquest, Encoding.UTF8, "text/json");

        //try
        //{

        //    HttpClient client = new HttpClient();
        //    client.MaxResponseContentBufferSize = 256000;
        //    client.BaseAddress = new Uri("http://192.168.0.102:88");

        //    string url = string.Format("/api/DeviceUsersApi/{0}", person.usuario);
        //    var response = await client.PutAsync(url, content);

        //    result = response.Content.ReadAsStringAsync().Result;

        //}
        //catch (Exception ex)
        //{

        //    Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();
        //    return;
        //}


        //Toast.MakeText(this, "Guardado Correctamente" , ToastLength.Long).Show();
        #endregion
    }
}

