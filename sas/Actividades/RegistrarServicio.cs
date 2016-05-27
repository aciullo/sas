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

namespace sas
{
    [Activity(Label = "Registrar Servicio", Theme = "@style/MyCustomTheme")]
    public class RegistrarServicio : Activity
    {
        private ServiciosModel servicio;
        private List<SasDatosModel> sasdatos;
        private SasDatosModel sasdatosBusqueda;
        EditText txtNroSolicitud;
        EditText txtNombrePaciente;
        EditText txtEdad;
        Button btnRegistroInicial;
        Button btnVolverBase;
        Button btnTranslado;
        Button btnRegistrarResultado;
        Button btnBuscar;
        TextView lblDestinoDesenlace;
        TextView lblDescrpcionDestinoDesenlace;
        EditText txtDestinoDesenlace;
        ProgressBar mProgress;

        UserSessionManager session;
        string IPCONN = "";


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

            //recibir datos de la actividad predecesora
            servicio = this.Intent.GetParcelableExtra("ServiciosDet") as ServiciosModel;
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
            btnRegistrarResultado.Click += BtnRegistrarResultado_Click;
            btnTranslado.Click += BtnTranslado_Click;

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
        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string codtabla="";
            var newActivity = new Intent(this, typeof(Buscar));
            newActivity.PutExtra("ServiciosDet", servicio);
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
                    btnRegistroInicial.Text = "Registrar Salida Base";
                    break;
                case ("002"):
                    btnRegistroInicial.Text = "Registrar Salida Base";
                    break;
                case "003":
                    btnRegistroInicial.Text = "Registrar Llegada a Servicio";
                    break;
                case "004":
                    btnRegistroInicial.Enabled = false;
                    btnVolverBase.Enabled = true;
                    btnTranslado.Enabled = true;

                    break;
                case "005":
                    btnRegistroInicial.Enabled = false;
                    btnVolverBase.Enabled = false;
                    btnTranslado.Enabled = false;
                    btnRegistrarResultado.Visibility = ViewStates.Visible;
                    if (string.IsNullOrEmpty(codInstitucionRecibido)&& (string.IsNullOrEmpty(codDesenlace)))
                    {
                        txtDestinoDesenlace.Visibility = ViewStates.Invisible;
                        lblDestinoDesenlace.Visibility = ViewStates.Invisible;
                        txtDestinoDesenlace.Visibility = ViewStates.Invisible;
                        btnRegistrarResultado.Text = "Registrar Llegada a Base";
                    }
                    else
                    {
                        txtDestinoDesenlace.Visibility = ViewStates.Visible;
                        lblDestinoDesenlace.Visibility = ViewStates.Visible;
                        txtDestinoDesenlace.Visibility = ViewStates.Visible;
                        //mostrar institucion
                        txtDestinoDesenlace.Text = codInstitucionRecibido;
                        await GetIndexDato(txtDestinoDesenlace.Text);
                        btnRegistrarResultado.Text = "Registrar Llegada a Institución";
                        txtDestinoDesenlace.Enabled = false;
                    }
                    break;
                case "006":
                    btnRegistroInicial.Enabled = false;
                    btnVolverBase.Enabled = false;
                    btnTranslado.Enabled = false;
                    btnRegistrarResultado.Visibility = ViewStates.Visible;

                    txtDestinoDesenlace.Visibility = ViewStates.Visible;
                    lblDestinoDesenlace.Visibility = ViewStates.Visible;
                    txtDestinoDesenlace.Visibility = ViewStates.Visible;

                    lblDestinoDesenlace.Text = "Institución";
                    txtDestinoDesenlace.Text = codInstitucionRecibido;
                    await GetIndexDato(txtDestinoDesenlace.Text);
                    txtDestinoDesenlace.Enabled = false;
                    btnRegistrarResultado.Text = "Registrar Salida de Institución";
                    break;
                case "007":
                    btnRegistroInicial.Enabled = false;
                    btnVolverBase.Enabled = false;
                    btnTranslado.Enabled = false;
                    btnRegistrarResultado.Visibility = ViewStates.Visible;

                    btnRegistrarResultado.Text = "Registrar Llegada a Base";

                    break;
                case "008":
                    btnRegistroInicial.Enabled = false;
                    btnVolverBase.Enabled = false;
                    btnTranslado.Enabled = false;
                    btnRegistrarResultado.Visibility = ViewStates.Visible;

                    btnRegistrarResultado.Text = "Registrar Desenlace";
                    txtDestinoDesenlace.Visibility = ViewStates.Visible;
                    lblDestinoDesenlace.Visibility = ViewStates.Visible;
                    txtDestinoDesenlace.Visibility = ViewStates.Visible;

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
            btnRegistrarResultado.Text = "Registrar Salida a Institución";
            btnRegistrarResultado.Visibility = ViewStates.Visible;
          

            txtDestinoDesenlace.Visibility = ViewStates.Visible;
            lblDestinoDesenlace.Visibility = ViewStates.Visible;
            txtDestinoDesenlace.Visibility = ViewStates.Visible;

            txtDestinoDesenlace.Enabled = true;

            lblDestinoDesenlace.Text = "Institución";

            txtDestinoDesenlace.Text = string.Empty;
            lblDescrpcionDestinoDesenlace.Text = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private  void BtnRegistrarResultado_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Confirmación");
            builder.SetMessage("Se encuetra a punto de actualizar el servicio. ¿Continuar?.");
            builder.SetCancelable(true);
            builder.SetPositiveButton("Si", delegate
            {

                mProgress.Indeterminate = true;
                mProgress.Visibility = ViewStates.Visible;
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
                    btnRegistrarResultado.Text = "Registrar Desenlace";

                    txtDestinoDesenlace.Visibility = ViewStates.Visible;
                    lblDestinoDesenlace.Visibility = ViewStates.Visible;
                    txtDestinoDesenlace.Visibility = ViewStates.Visible;

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
                        var regservicio2 = new ABMServicioModel
                        {
                            id_Solicitud = servicio.id_Solicitud,
                            NumeroSolicitud = servicio.NumeroSolicitud,
                            codServicioFinal = "Null",
                            codProductoFinal = txtDestinoDesenlace.Text
                        };
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
                        var regservicio = new ABMServicioModel
                        {
                            id_Solicitud = servicio.id_Solicitud,
                            NumeroSolicitud = servicio.NumeroSolicitud,
                            codServicioFinal = txtDestinoDesenlace.Text,
                            codProductoFinal = "Null"

                        };
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
                    hora_Llegada = string.Format("{0:HH:mm}", System.DateTime.Now),
                    idestado = idestado

                };

                await GuardarDatos(regservicio);
                mProgress.Visibility = ViewStates.Gone;
            }
        }
        private void BtnVolverBase_Click(object sender, EventArgs e)
        {


            btnRegistrarResultado.Text = "Registrar Salida a Base";
            btnRegistrarResultado.Visibility= ViewStates.Visible;
            btnVolverBase.Enabled = false;
            btnTranslado.Enabled = false;

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
                case "Registrar Salida Base":
                    idestado = "003";
                    // btnRegistroInicial.BackgroundTintList= Color.Navy;
                    btnRegistroInicial.Text = "Registrar Llegada a Servicio";

                    break;
                case "Registrar Llegada a Servicio":
                    idestado = "004";
                    btnRegistroInicial.Enabled = false;
                    btnVolverBase.Enabled = true;
                    btnTranslado.Enabled = true;


                    break;
                default:
                    idestado = "";
                    break;
            }



            var regservicio = new RegistrarServicioModel
            {
                id_Solicitud = servicio.id_Solicitud,
                NumeroSolicitud = servicio.NumeroSolicitud,
                hora_Llegada = string.Format("{0:HH:mm}", System.DateTime.Now),
                idestado = idestado

            };

            await GuardarDatos(regservicio);

            mProgress.Visibility = ViewStates.Gone;

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

                string url = string.Format("/api/UpdServiciosApi?idsolicitud={0}&codestado={1}&hora={2}", regservicio.id_Solicitud, regservicio.idestado, regservicio.hora_Llegada);
                var response = await client.GetAsync(url);
                result = response.Content.ReadAsStringAsync().Result;
                //Items = JsonConvert.DeserializeObject <List<Personas>> (result);

                if (result.Contains("Error"))
                {
                    Toast.MakeText(this, "Error", ToastLength.Long).Show();
                   
                    //using (var datos = new DataAccess())
                    //{
                    //    datos.InsertEmpleado(regservicio);
                    //}


                }


            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();
                
                //using (var datos = new DataAccess())
                //{
                //    datos.InsertEmpleado(regservicio);
                //}

              


                return;
            }


            //waitActivityIndicator.IsRunning = false;

            Toast.MakeText(this, "Registro guardado Correctamtne", ToastLength.Long).Show();
           

        }


        private async Task actualizarInstitucionDesenlace(ABMServicioModel servTranslado)
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

                if (servTranslado.codProductoFinal == "Null")
                {
                    var url = string.Format("/api/ABMServiciosApi?idsolicitud={0}&nrosolicitud={1}&destino={2}", servTranslado.id_Solicitud, servTranslado.NumeroSolicitud, servTranslado.codServicioFinal);
                    response = await client.GetAsync(url);
                }
                else
                {
                    var url = (string.Format("/api/ABMServiciosApi?idsolicitud={0}&nrosolicitud={1}&destino={2}&desenlace={3}", servTranslado.id_Solicitud, servTranslado.NumeroSolicitud, servTranslado.codServicioFinal, servTranslado.codProductoFinal));
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

                    //using (var datos = new DataAccess())
                    //{
                    //    datos.InsertEmpleado(regservicio);
                    //}


                }
            }
            catch (Exception ex)
            {
                //guaardar localmente
                Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();
                // transladoButton.IsEnabled = true;
                // waitActivityIndicator.IsRunning = false;
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
            //waitActivityIndicator.IsRunning = false;
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
            //waitActivityIndicator.IsRunning = false;




        }
    }
}