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
using sas.Actividades;

namespace sas
{
    [Activity(Label = "Servicios Detalle")]
    public class ServiciosDetalle : Activity
    {
        private ServiciosModel servicio;
        private MotivosModel motivo;

        EditText txtNroSolicitud;
        EditText txtNombrePaciente;
        EditText txtEdad;
        EditText txtSolicitante;
        EditText txtDireccion1;
        EditText txtDireccion2;
        EditText txtMotivo1;
        EditText txtMotivo2;
        EditText txtMotivo3;
        EditText txtOtroMotivo;
        Button btnIniciarServicio;
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
            txtDireccion2 = FindViewById<EditText>(Resource.Id.txtDireccion2);
            txtMotivo1 = FindViewById<EditText>(Resource.Id.txtMotivo1);
            txtMotivo2 = FindViewById<EditText>(Resource.Id.txtMotivo2);
            txtMotivo3 = FindViewById<EditText>(Resource.Id.txtMotivo3);
            txtOtroMotivo = FindViewById<EditText>(Resource.Id.txtOtroMotivo);
            btnIniciarServicio = FindViewById<Button>(Resource.Id.btnIniciarServicio);

            servicio = this.Intent.GetParcelableExtra("ServiciosDet") as ServiciosModel;

            txtNroSolicitud.Text = servicio.NumeroSolicitud.ToString();
            txtNombrePaciente.Text = servicio.nombrePaciente;
            txtEdad.Text = servicio.edadPaciente.ToString();
            txtSolicitante.Text = servicio.nombreSolicitante;
            txtDireccion1.Text = servicio.direccionReferecia;
            txtDireccion2.Text = servicio.direccionReferecia2;
            txtOtroMotivo.Text = servicio.OtroMotivo;


            if (!string.IsNullOrEmpty(servicio.codMotivo1))
            {
                LoadMotivo1();
            }

            if (!string.IsNullOrEmpty(servicio.codMotivo2))
            {
                LoadMotivo2();
            }
            if (!string.IsNullOrEmpty(servicio.codMotivo3))
            {
                LoadMotivo3();
            }

            btnIniciarServicio.Click += BtnIniciarServicio_Click;

        }

        private void BtnIniciarServicio_Click(object sender, EventArgs e)
        {
            var newActivity = new Intent(this, typeof(RegistrarServicio));
            newActivity.PutExtra("ServiciosDet", servicio);
            StartActivity(newActivity);
            Finish();
        }

        private async void LoadMotivo1()
        {
            string result;
          

            try
            {

                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;

                client.BaseAddress = new Uri("http://192.168.0.13");
                //var uri = new Uri (string.Format ("http://192.168.0.13/sas_Futura/api/sas_ServiciosApi/{0}/{1}/{2}", deviceUser.codMovil,"001","P" ));

                string url = string.Format("/sas_Futura/api/MotivosModelsApi/{0}", servicio.codMotivo1.TrimEnd());
                var response = await client.GetAsync(url);
                result = response.Content.ReadAsStringAsync().Result;
                //Items = JsonConvert.DeserializeObject <List<Personas>> (result);

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();

                return;
            }

            try
            {
              
                motivo = JsonConvert.DeserializeObject<MotivosModel>(result);
                txtMotivo1.Text = motivo.descripcionMotivo;
                //motivosListView.ItemsSource = motivo;
                //se carga el picker recorriendo


            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();
                
                return;
            }
           

        }


        private async void LoadMotivo2()
        {
            string result;
          

            try
            {

                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;

                client.BaseAddress = new Uri("http://192.168.0.13");
                //var uri = new Uri (string.Format ("http://192.168.0.13/sas_Futura/api/sas_ServiciosApi/{0}/{1}/{2}", deviceUser.codMovil,"001","P" ));

                string url = string.Format("/sas_Futura/api/MotivosModelsApi/{0}", servicio.codMotivo2.TrimEnd());
                var response = await client.GetAsync(url);
                result = response.Content.ReadAsStringAsync().Result;
                //Items = JsonConvert.DeserializeObject <List<Personas>> (result);

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();
                return;
            }

            try
            {
               
                motivo = JsonConvert.DeserializeObject<MotivosModel>(result);
                txtMotivo2.Text = motivo.descripcionMotivo;
                //motivosListView.ItemsSource = motivo;
                //se carga el picker recorriendo


            }
            catch (Exception ex)
            {
                
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                return;
            }
         

        }


        private async void LoadMotivo3()
        {
            string result;
           

            try
            {

                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;

                client.BaseAddress = new Uri("http://192.168.0.13");
                //var uri = new Uri (string.Format ("http://192.168.0.13/sas_Futura/api/sas_ServiciosApi/{0}/{1}/{2}", deviceUser.codMovil,"001","P" ));

                string url = string.Format("/sas_Futura/api/MotivosModelsApi/{0}", servicio.codMotivo3.TrimEnd());
                var response = await client.GetAsync(url);
                result = response.Content.ReadAsStringAsync().Result;
                //Items = JsonConvert.DeserializeObject <List<Personas>> (result);

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();
               
                return;
            }

            try
            {
                
                motivo = JsonConvert.DeserializeObject<MotivosModel>(result);
                txtMotivo3.Text = motivo.descripcionMotivo;
                //motivosListView.ItemsSource = motivo;
                //se carga el picker recorriendo


            }
            catch (Exception ex)
            {
               
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                return;
            }
         

        }
    }
}