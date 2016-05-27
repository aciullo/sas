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
using sas.Clases;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace sas.Actividades
{
    [Activity(Label = "Buscar")]
    public class Buscar : Activity
    {
        private ServiciosModel servicio;
        private List<SasDatosModel> sasdatos;
        private List<SasDatosModel> resultado;
        string codtabla;
        EditText txtBusqueda;
        ListView lstSasDatos;
        UserSessionManager session;
        string IPCONN = "";


        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.BuscarLayout);
            // Create your application here

            //instanciar los controles
            txtBusqueda = FindViewById<EditText>(Resource.Id.txtBusqueda);
            lstSasDatos = FindViewById<ListView>(Resource.Id.lstSasDatos);

            //asignar eventos
           
            lstSasDatos.ItemClick += LstSasDatos_ItemClick;
           // txtBusqueda.KeyPress += TxtBusqueda_KeyPress;
            //recibir datos servicios
            servicio= this.Intent.GetParcelableExtra("ServiciosDet") as ServiciosModel;
            //recibir codigo tabla
            codtabla = this.Intent.GetStringExtra("codtabla");

            session = new UserSessionManager(this);
            IPCONN = session.getAccessConn();


            await GetIndexDato(codtabla);


            txtBusqueda.TextChanged += async (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                // filter on text changed
                var searchTerm = txtBusqueda.Text;
                if (string.IsNullOrEmpty(txtBusqueda.Text))
                {
                    await GetIndexDato(codtabla);
                    return;
                }
                //Toast.MakeText(this, txtDestinoDesenlace.Text, ToastLength.Short).Show();
                resultado = sasdatos.FindAll(x => (x.descripcion.ToUpper()).Contains(txtBusqueda.Text.ToUpper()));

                lstSasDatos.ChoiceMode = ChoiceMode.Single;
                lstSasDatos.Adapter = new BusquedaAdapter(this, resultado);
               
            };
        }

        //private async void TxtBusqueda_KeyPress(object sender, View.KeyEventArgs e)
        //{
        //    e.Handled = false;
        //    if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
        //    {
        //        if (string.IsNullOrEmpty(txtBusqueda.Text))
        //        {
        //            await GetIndexDato(codtabla);
        //            return;
        //        }
        //        //Toast.MakeText(this, txtDestinoDesenlace.Text, ToastLength.Short).Show();
        //       //var resultado = sasdatos.FindAll(x => x.descripcion.Contains(txtBusqueda.Text));

        //        //lstSasDatos.ChoiceMode = ChoiceMode.Single;
        //        //lstSasDatos.Adapter = new BusquedaAdapter(this, resultado);
        //        lstSasDatos.SetFilterText( txtBusqueda.Text);
        //        e.Handled = true;
        //    }
        //}

        private void LstSasDatos_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {

            SasDatosModel t;
            Intent intent = new Intent(this, typeof(RegistrarServicio));

            if (resultado != null)
            {

                if (resultado.Count < sasdatos.Count  )
                {
                    t = resultado[e.Position];
                   

                }
                else
                {

                    t = sasdatos[e.Position];

                }

               

                intent.PutExtra("ServiciosDet", servicio);
                intent.PutExtra("sasDatos", t);

                //intent.SetFlags(ActivityFlags.ReorderToFront);

                SetResult(Result.Ok, intent);

                //StartActivity(intent);

                // FinishActivity(0);
                Finish();
                return;
            }

            t = sasdatos[e.Position];

            intent.PutExtra("ServiciosDet", servicio);
            intent.PutExtra("sasDatos", t);

            //intent.SetFlags(ActivityFlags.ReorderToFront);

            SetResult(Result.Ok, intent);

            Finish();
            return;


        }

      
        async Task GetIndexDato( string codtabla,string Id=null)
        {

            string result;

            try
            {

                HttpClient client = new HttpClient();
                client.MaxResponseContentBufferSize = 256000;


                //client.BaseAddress = new Uri("http://181.120.121.221:88");
                client.BaseAddress = new Uri(IPCONN);
                //var uri = new Uri (string.Format ("http://181.120.121.221:88/api/sas_ServiciosApi/{0}/{1}/{2}", deviceUser.codMovil,"001","P" ));


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

            lstSasDatos.ChoiceMode = ChoiceMode.Single;

            lstSasDatos.Adapter = new BusquedaAdapter(this, sasdatos);

        }
    }

   }