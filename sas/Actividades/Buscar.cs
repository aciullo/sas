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
using sas.Core;
namespace sas.Actividades
{
    [Activity(Label = "Buscar", Theme = "@style/MyCustomTheme")]
    public class Buscar : Activity
    {
        // private ServiciosModel servicio;
        private IList<SasDatosItem> busqueda;
        private List<SasDatosModel> sasdatos;
        private IList<SasDatosItem> resultado;
        string codtabla;
        EditText txtBusqueda;
        ListView lstSasDatos;
        UserSessionManager session;
        string IPCONN = "";


        protected override void OnCreate(Bundle savedInstanceState)
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
          //  servicio= this.Intent.GetParcelableExtra("ServiciosDet") as ServiciosModel;
            //recibir codigo tabla
            codtabla = this.Intent.GetStringExtra("codtabla");

            session = new UserSessionManager(this);
            IPCONN = session.getAccessConn();


            //await GetIndexDato(codtabla);
            busqueda = SasDatosManager.GetTasks(codtabla);
            lstSasDatos.ChoiceMode = ChoiceMode.Single;
            lstSasDatos.Adapter = new BusquedaAdapter(this, busqueda);

            txtBusqueda.TextChanged +=  (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                // filter on text changed
                var searchTerm = txtBusqueda.Text;
                if (string.IsNullOrEmpty(txtBusqueda.Text))
                {
                    // await GetIndexDato(codtabla);
                    busqueda = SasDatosManager.GetTasks(codtabla);
                    lstSasDatos.ChoiceMode = ChoiceMode.Single;
                    lstSasDatos.Adapter = new BusquedaAdapter(this, busqueda);
                    resultado.Clear();
                    return;
                }
                //Toast.MakeText(this, txtDestinoDesenlace.Text, ToastLength.Short).Show();
                resultado = busqueda.Where(x => (x.descripcion.ToUpper()).Contains(txtBusqueda.Text.ToUpper())).ToList();

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

            SasDatosItem t;
            Intent intent = new Intent(this, typeof(RegistrarServicio));
            Bundle valuesForActivity = new Bundle();
            if (resultado != null)
            {

                if (resultado.Count!=0 && resultado.Count < busqueda.Count  )
                {
                    t = resultado[e.Position];
                   

                }
                else
                {

                    t = busqueda[e.Position];

                }

                

              //  intent.PutExtra("ServiciosDet", servicio);
                //intent.PutExtra("sasDatos", t);
               
                valuesForActivity.PutInt("sasDatos", t.ID);
                //intent.SetFlags(ActivityFlags.ReorderToFront);
                intent.PutExtras(valuesForActivity);
                SetResult(Result.Ok, intent);

                //StartActivity(intent);

                // FinishActivity(0);
                Finish();
                return;
            }

            t = busqueda[e.Position];

            //  intent.PutExtra("ServiciosDet", servicio);
            //intent.PutExtra("sasDatos", t);

            //intent.SetFlags(ActivityFlags.ReorderToFront);
           
            valuesForActivity.PutInt("sasDatos", t.ID);

            intent.PutExtras(valuesForActivity);

            SetResult(Result.Ok, intent);

            Finish();
            return;


        }

      
        //async Task GetIndexDato( string codtabla,string Id=null)
        //{

        //    string result;

        //    try
        //    {

        //        HttpClient client = new HttpClient();
        //        client.MaxResponseContentBufferSize = 256000;


        //        //client.BaseAddress = new Uri("http://181.120.121.221:88");
        //        client.BaseAddress = new Uri(IPCONN);
        //        //var uri = new Uri (string.Format ("http://181.120.121.221:88/api/sas_ServiciosApi/{0}/{1}/{2}", deviceUser.codMovil,"001","P" ));


        //        string url = string.Format("/api/SasDatosApi?idtabla={0}&codigo={1}", codtabla, Id);
        //        var response = await client.GetAsync(url);
        //        result = response.Content.ReadAsStringAsync().Result;
        //        //Items = JsonConvert.DeserializeObject <List<Personas>> (result);



        //    }
        //    catch (Exception ex)
        //    {

        //        Toast.MakeText(this, "No hay conexión intente más tarde", ToastLength.Long).Show();

        //        return;
        //    }


        //    sasdatos = JsonConvert.DeserializeObject<List<SasDatosModel>>(result);

        //    lstSasDatos.ChoiceMode = ChoiceMode.Single;

        //    lstSasDatos.Adapter = new BusquedaAdapter(this, sasdatos);

        //}
    }

   }