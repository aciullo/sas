using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using sas.Clases;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace sas.Actividades
{
    public class BuscarFg : Fragment
    {
        private ServiciosModel servicio;
        private List<SasDatosModel> sasdatos;

        string codtabla;
        EditText txtBusqueda;
        ListView lstSasDatos;
        UserSessionManager session;
        string IPCONN = "";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            SetContentView(Resource.Layout.BuscarLayout);
            // Create your application here

            //instanciar los controles
            txtBusqueda = FindViewById<EditText>(Resource.Id.txtBusqueda);
            lstSasDatos = FindViewById<ListView>(Resource.Id.lstSasDatos);

            //asignar eventos

            lstSasDatos.ItemClick += LstSasDatos_ItemClick;
            txtBusqueda.KeyPress += TxtBusqueda_KeyPress;
            //recibir datos servicios
            servicio = this.Intent.GetParcelableExtra("ServiciosDet") as ServiciosModel;
            //recibir codigo tabla
            codtabla = this.Intent.GetStringExtra("codtabla");

            session = new UserSessionManager(this);
            IPCONN = session.getAccessConn();


            await GetIndexDato(codtabla);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }
        private void TxtBusqueda_KeyPress(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                //Toast.MakeText(this, txtDestinoDesenlace.Text, ToastLength.Short).Show();
                var resultado = sasdatos.FindAll(x => x.descripcion.Contains(txtBusqueda.Text));
                lstSasDatos.ChoiceMode = ChoiceMode.Single;
                lstSasDatos.Adapter = new BusquedaAdapter(this, resultado);

                e.Handled = true;
            }
        }

        private void LstSasDatos_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var t = sasdatos[e.Position];

            Intent intent = new Intent();

            intent.SetClass(this.Context, typeof(RegistrarServicio));
            intent.PutExtra("ServiciosDet", servicio);
            intent.PutExtra("sasDatos", t);

            // intent.SetFlags(ActivityFlags.ReorderToFront);

           // SetResult(Result.Ok, intent);

           
            StartActivity(intent);

          

        }


        async Task GetIndexDato(string codtabla, string Id = null)
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