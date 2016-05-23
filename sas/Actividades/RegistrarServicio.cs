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

namespace sas.Actividades
{
    [Activity(Label = "Registrar Servicio")]
    public class RegistrarServicio : Activity
    {
        private ServiciosModel servicio;
        private List<SasDatosModel> sasdatos;
        EditText txtNroSolicitud;
        EditText txtNombrePaciente;
        EditText txtEdad;
        Button btnRegistroInicial;
        Button btnVolverBase;
        Button btnTranslado;
        Button btnRegistrarResultado;
        TextView lblDestinoDesenlace;
        TextView lblDescrpcionDestinoDesenlace;
        EditText txtDestinoDesenlace;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.RegistrarServiciolayout);

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

            servicio = this.Intent.GetParcelableExtra("ServiciosDet") as ServiciosModel;

            txtNroSolicitud.Text = servicio.NumeroSolicitud.ToString();
            txtNombrePaciente.Text = servicio.nombrePaciente;
            txtEdad.Text = servicio.edadPaciente.ToString();

            btnRegistroInicial.Click += BtnRegistroInicial_Click;
            btnVolverBase.Click += BtnVolverBase_Click;
            btnRegistrarResultado.Click += BtnRegistrarResultado_Click;
            btnTranslado.Click += BtnTranslado_Click;

            txtDestinoDesenlace.KeyPress += (object sender, View.KeyEventArgs e) => {
                e.Handled = false;
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    //Toast.MakeText(this, txtDestinoDesenlace.Text, ToastLength.Short).Show();
                    GetIndexDato(txtDestinoDesenlace.Text);
                    e.Handled = true;
                }
            };

            txtDestinoDesenlace.FocusChange += TxtDestinoDesenlace_FocusChange;
        }

        private void TxtDestinoDesenlace_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                //Toast.MakeText(this, txtDestinoDesenlace.Text, ToastLength.Short).Show();
                GetIndexDato(txtDestinoDesenlace.Text);
               
            }
        }

        private void BtnTranslado_Click(object sender, EventArgs e)
        {
            btnVolverBase.Enabled = false;
            btnTranslado.Enabled = false;
            btnRegistrarResultado.Text = "Registrar Salida a Instituci�n";
            btnRegistrarResultado.Visibility = ViewStates.Visible;
          

            txtDestinoDesenlace.Visibility = ViewStates.Visible;
            lblDestinoDesenlace.Visibility = ViewStates.Visible;
            txtDestinoDesenlace.Visibility = ViewStates.Visible;

            txtDestinoDesenlace.Enabled = true;

            lblDestinoDesenlace.Text = "Ingresar Instituci�n";

            txtDestinoDesenlace.Text = string.Empty;
            lblDescrpcionDestinoDesenlace.Text = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async   void BtnRegistrarResultado_Click(object sender, EventArgs e)
        {
            string idestado = "";

            switch (btnRegistrarResultado.Text)
            {
                case "Registrar Salida a Base":
                    idestado = "005";
                    //volverBaseButton.BackgroundColor = Color.Green;
                    btnRegistrarResultado.Text = "Servicio Finalizado";

                    break;

                case "Servicio Finalizado":
                    idestado = "009";
                    //volverBaseButton.BackgroundColor = Color.Purple;
                    btnRegistrarResultado.Text = "Registrar Llegada a Base";

                    break;
                case "Registrar Llegada a Base":
                    idestado = "008";
                    // volverBaseButton.BackgroundColor = Color.Maroon;
                    btnRegistrarResultado.Text = "Registrar Desenlace";

                    txtDestinoDesenlace.Visibility = ViewStates.Visible;
                    lblDestinoDesenlace.Visibility = ViewStates.Visible;
                    txtDestinoDesenlace.Visibility = ViewStates.Visible;

                    txtDestinoDesenlace.Enabled = true;

                    lblDestinoDesenlace.Text = "Ingresar desenlace";

                    txtDestinoDesenlace.Text = string.Empty;
                    lblDescrpcionDestinoDesenlace.Text = string.Empty;
                    
                    //volverBaseButton.IsEnabled = false;
                    break;
                case "Registrar Desenlace":
                    await GetIndexDato(txtDestinoDesenlace.Text);

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
                case "Registrar Salida a Instituci�n":
                    idestado = "005";
                    await GetIndexDato(txtDestinoDesenlace.Text);
                    if (string.IsNullOrEmpty(txtDestinoDesenlace.Text))
                    {

                        Toast.MakeText(this, "Debe ingresar una instituci�n", ToastLength.Long).Show();
                      
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
                    btnRegistrarResultado.Text = "Registrar Llegada a Instituci�n";
                    txtDestinoDesenlace.Enabled = false;
                    break;
                case "Registrar Llegada a Instituci�n":
                    idestado = "006";
                    // regTransladoButton.BackgroundColor = Color.Teal;
                    btnRegistrarResultado.Text = "Registrar Salida de Instituci�n";
                    break;
                case "Registrar Salida de Instituci�n":
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

                GuardarDatos(regservicio);
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

                    break;
            }



            var regservicio = new RegistrarServicioModel
            {
                id_Solicitud = servicio.id_Solicitud,
                NumeroSolicitud = servicio.NumeroSolicitud,
                hora_Llegada = string.Format("{0:HH:mm}", System.DateTime.Now),
                idestado = idestado

            };

            GuardarDatos(regservicio);
        }

        #region "Metodos actualizacion BD"
        private async void GuardarDatos(RegistrarServicioModel regservicio)
        {

            string result;

         

            try
            {


                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;


                client.BaseAddress = new Uri("http://192.168.0.13");
                //var uri = new Uri (string.Format ("http://192.168.0.13/sas_Futura/api/sas_ServiciosApi/{0}/{1}/{2}", deviceUser.codMovil,"001","P" ));

                string url = string.Format("/sas_Futura/api/UpdServiciosApi?idsolicitud={0}&codestado={1}&hora={2}", regservicio.id_Solicitud, regservicio.idestado, regservicio.hora_Llegada);
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
                Toast.MakeText(this, "No hay conexi�n intente m�s tarde", ToastLength.Long).Show();
                
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
                //	client.BaseAddress = new Uri ("http://192.168.0.13");

                System.Net.Http.HttpResponseMessage response;

                if (servTranslado.codProductoFinal == "Null")
                {
                    var uri = new Uri(string.Format("http://192.168.0.13/sas_Futura/api/ABMServiciosApi?idsolicitud={0}&nrosolicitud={1}&destino={2}", servTranslado.id_Solicitud, servTranslado.NumeroSolicitud, servTranslado.codServicioFinal));
                    response = await client.GetAsync(uri);
                }
                else
                {
                    var uri = new Uri(string.Format("http://192.168.0.13/sas_Futura/api/ABMServiciosApi?idsolicitud={0}&nrosolicitud={1}&destino={2}&desenlace={3}", servTranslado.id_Solicitud, servTranslado.NumeroSolicitud, servTranslado.codServicioFinal, servTranslado.codProductoFinal));
                    response = await client.GetAsync(uri);

                }
                //string url = string.Format ("/sas_Futura/api/ABMServiciosApi?idsolicitud={0}&nrosolicitud={1}&destino={2}&IdProductoFinal={3}", servTranslado.id_Solicitud,servTranslado.NumeroSolicitud,servTranslado.codServicioFinal, servTranslado.codProductoFinal);
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
                Toast.MakeText(this, "No hay conexi�n intente m�s tarde", ToastLength.Long).Show();
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


                client.BaseAddress = new Uri("http://192.168.0.13");
                //var uri = new Uri (string.Format ("http://192.168.0.13/sas_Futura/api/sas_ServiciosApi/{0}/{1}/{2}", deviceUser.codMovil,"001","P" ));
                string codtabla = "";
                if (lblDestinoDesenlace.Text == "Ingresar desenlace")
                {
                    codtabla = "07";
                }
                else
                {
                    codtabla = "06";
                }
                string url = string.Format("/sas_Futura/api/SasDatosApi?idtabla={0}&codigo={1}", codtabla, Id);
                var response = await client.GetAsync(url);
                result = response.Content.ReadAsStringAsync().Result;
                //Items = JsonConvert.DeserializeObject <List<Personas>> (result);



            }
            catch (Exception ex)
            {

                Toast.MakeText(this, "No hay conexi�n intente m�s tarde", ToastLength.Long).Show();





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
                //await DisplayAlert("Error", "C�digo no existe", "Aceptar");
                Toast.MakeText(this, "C�digo no existe", ToastLength.Long).Show();
            }
            //waitActivityIndicator.IsRunning = false;




        }
    }
}