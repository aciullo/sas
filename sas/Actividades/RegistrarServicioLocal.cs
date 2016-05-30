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

namespace sas.Actividades
{
    [Activity(Label = "Registrar Servicio Local", Theme = "@style/MyCustomTheme")]
    public class RegistrarServicioLocal : Activity
    {

        ListView listView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.RegistroServiciosLocal); // loads the HomeScreen.axml as this activity's view
            listView = FindViewById<ListView>(Resource.Id.List); // get reference to the ListView in the layout
                                                                 // populate the listview with data

            using (var datos = new DataAccess())
            {
                listView.Adapter = new RegistroServicioLocalAdapter(this, datos.GetServicios());
            }


            listView.ItemClick += ListView_ItemClick;

        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
              var t = new List<RegistrarServicioModel>();
                        using (var datos = new DataAccess())
                        {
                            t = datos.GetServicios();
                        }
          
                       var itemEliminar= t[e.Position];

                        using (var datos = new DataAccess())
                        {
                            datos.DeleteServicio(itemEliminar);
                        }
            }
            catch (Exception ex) {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();

                return;
            }
            Toast.MakeText(this, "Servicio borrado correctamente", ToastLength.Long).Show();
            using (var datos = new DataAccess())
            {
                listView.Adapter = new RegistroServicioLocalAdapter(this, datos.GetServicios());
            }
        }
    }
}