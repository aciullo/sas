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
using sas;
using sas.Core;
namespace sas.Clases
{
    class DetServicioLocalAdapter : BaseAdapter <ServicioItem>
    {
        IList<ServicioItem> items;
        Activity context;
        public DetServicioLocalAdapter(Activity context, IList<ServicioItem> items)
        : base()
         {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }

        public override ServicioItem this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];

            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.ServicioLocalView, null);
            view.FindViewById<TextView>(Resource.Id.Text1).Text =  item.ID.ToString();
            view.FindViewById<TextView>(Resource.Id.Text2).Text = "Id solicitud:  " + item.id_Solicitud.ToString();
            view.FindViewById<TextView>(Resource.Id.Text3).Text = "Enviado: " + item.Enviado.ToString();
            view.FindViewById<TextView>(Resource.Id.Text4).Text = "Hora Estado: " + item.HoraEstado;
            view.FindViewById<TextView>(Resource.Id.Text5).Text = "Cod Estado: " + item.codEstado;
            view.FindViewById<TextView>(Resource.Id.Text6).Text = "Desenlace:  " + item.codDesenlace;
            view.FindViewById<TextView>(Resource.Id.Text7).Text = "Institucion: " + item.codInstitucion;
            view.FindViewById<TextView>(Resource.Id.Text8).Text = "Estado:  " + item.Estado;
            view.FindViewById<TextView>(Resource.Id.Text9).Text = "GeoData: " + item.GeoData;



            return view;
        }

    }
}