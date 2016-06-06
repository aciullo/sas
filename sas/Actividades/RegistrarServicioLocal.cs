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
using sas.Core;
namespace sas.Actividades
{
    [Activity(Label = "Registrar Servicio Local", Theme = "@style/MyCustomTheme")]
    public class RegistrarServicioLocal : Activity
    {

        ListView listView;
        ListView listView2;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.RegistroServiciosLocal); // loads the HomeScreen.axml as this activity's view
            listView = FindViewById<ListView>(Resource.Id.List); // get reference to the ListView in the layout
            listView2 = FindViewById<ListView>(Resource.Id.List2);                                                   // populate the listview with data

            int ID = Intent.Extras.GetInt("ServiciosDet");

           

            //listView.Adapter = new RegistroServicioLocalAdapter(this, ServicioManager.GetTasks());
            listView.Adapter = new DetServicioLocalAdapter (this, ServicioItemManager.GetItemByForeingID(ID));


         

        }
        
    }
}