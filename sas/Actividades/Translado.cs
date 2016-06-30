using System;
using System.Collections.Generic;
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
using sas.Core;
using Android.Locations;
using Android.Util;

namespace sas.Actividades
{
    [Activity(Label = "Translado", Theme = "@style/MyCustomTheme", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class Translado : Activity, ILocationListener
    {
        EditText txtNroSolicitud;
        EditText txtNombrePaciente;
        EditText txtEdad;
        Button btnRegistroInicial;
        Button btnVolverBase;
        Button btnTranslado;
        Button btnRegistrarResultado;
        Button btnBuscar;
        Button btnRegistroLocal;
        TextView lblDestinoDesenlace;
        TextView lblDescrpcionDestinoDesenlace;
        EditText txtDestinoDesenlace;
        ProgressBar mProgress;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }
    }
}