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
namespace sas
{
    [Activity(Label = "Servicios Detalle", Theme = "@style/MyCustomTheme")]
    public class ServiciosDetalle : Activity
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
        protected override async void OnCreate(Bundle savedInstanceState)
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

            txtMotivo1.Text = txtMotivo1.Text + ", " + servicio.OtroMotivo;

            btnIniciarServicio.Click += BtnIniciarServicio_Click;

        }

        private void BtnIniciarServicio_Click(object sender, EventArgs e)
        {
            var newActivity = new Intent(this, typeof(RegistrarServicio));
            // newActivity.PutExtra("ServiciosDet", servicio.ID);
            Bundle valuesForActivity = new Bundle();
            valuesForActivity.PutInt("ServiciosDet", ID);
            newActivity.PutExtras(valuesForActivity);

            StartActivity(newActivity);
            Finish();
        }

        private async Task LoadMotivo1()
        {
            string result;
          

            try
            {

                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;

                client.BaseAddress = new Uri("http://181.120.121.221:88");
                //var uri = new Uri (string.Format ("http://181.120.121.221:88/api/sas_ServiciosApi/{0}/{1}/{2}", deviceUser.codMovil,"001","P" ));

                string url = string.Format("/api/MotivosModelsApi/{0}", servicio.codMotivo1.TrimEnd());
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
                txtMotivo1.Text = txtMotivo1.Text + ", " + motivo.descripcionMotivo;
                //motivosListView.ItemsSource = motivo;
                //se carga el picker recorriendo


            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();
                
                return;
            }
           

        }


        private async  Task LoadMotivo2()
        {
            string result;
          

            try
            {

                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;

                client.BaseAddress = new Uri("http://181.120.121.221:88");
                //var uri = new Uri (string.Format ("http://181.120.121.221:88/api/sas_ServiciosApi/{0}/{1}/{2}", deviceUser.codMovil,"001","P" ));

                string url = string.Format("/api/MotivosModelsApi/{0}", servicio.codMotivo2.TrimEnd());
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
                txtMotivo1.Text = txtMotivo1.Text + ", " + motivo.descripcionMotivo;
                //motivosListView.ItemsSource = motivo;
                //se carga el picker recorriendo


            }
            catch (Exception ex)
            {
                
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                return;
            }
         

        }


        private async Task LoadMotivo3()
        {
            string result;
           

            try
            {

                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;

                client.BaseAddress = new Uri("http://181.120.121.221:88");
                //var uri = new Uri (string.Format ("http://181.120.121.221:88/api/sas_ServiciosApi/{0}/{1}/{2}", deviceUser.codMovil,"001","P" ));

                string url = string.Format("/api/MotivosModelsApi/{0}", servicio.codMotivo3.TrimEnd());
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
                txtMotivo1.Text = txtMotivo1.Text + ", " + motivo.descripcionMotivo;
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