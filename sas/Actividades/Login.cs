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


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

           // ActionBar.Title = "Ingreso al sistema";
           
            // Set our view from the "main" layout resource

            SetContentView(Resource.Layout.LoginLayout);

            // Get our button from the layout resource,
            // and attach an event to it
            //

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
                    Intent newActivity = new Intent(this, typeof(Servicios));
                    Bundle valuesForActivity = new Bundle();
                    valuesForActivity.PutInt("GPS", 0);
                    newActivity.PutExtras(valuesForActivity);
                    StartActivity(newActivity);
                    Finish();
            }
          
        }

        private async void BtnIngresar_Click(object sender, EventArgs e)
        {


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
                btnIngresar.Enabled = false;
                Toast.MakeText(this, "Procesando petición de ingreso", ToastLength.Long).Show();

                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;

                //client.BaseAddress = new Uri ("http://181.120.121.221:88");
                client.BaseAddress = new Uri(IPCONN);


               // var uri = new Uri(string.Format("http://181.120.121.221:88/api/DeviceUsersApi/{0}/{1}", txtUsuario.Text, txtClave.Text));

                string url = string.Format ("/api/DeviceUsersApi/{0}/{1}", txtUsuario.Text, txtClave.Text);

                var response = await client.GetAsync(url);

                var result = response.Content.ReadAsStringAsync();
                //Items = JsonConvert.DeserializeObject <List<Personas>> (result);



                if (string.IsNullOrEmpty(result.Result) || result.Result == "null")
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

              //  Toast.MakeText(this, "Ingresando...", ToastLength.Short).Show();

              

                var deviceUser = JsonConvert.DeserializeObject<List<DeviceUserModel>>(result.Result);


                session.createLoginSession((deviceUser[0].nombres + " " + deviceUser[0].apellidos), deviceUser[0].codMovil);

                StartService(new Intent("com.sas.searchpending"));
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

