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
    class DetServicioLocalAdapter : BaseAdapter <ServicioLocal>
    {
        IList<ServicioLocal> items;
        Activity context;
        public DetServicioLocalAdapter(Activity context, IList<ServicioLocal> items)
        : base()
         {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }

        public override ServicioLocal this[int position]
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
            view.FindViewById<TextView>(Resource.Id.Text1).Text = item.ID.ToString();
            view.FindViewById<TextView>(Resource.Id.Text2).Text = item.id_Solicitud.ToString();
            view.FindViewById<TextView>(Resource.Id.Text3).Text = item.codMovil;
            view.FindViewById<TextView>(Resource.Id.Text4).Text = item.HoraEstado;
            view.FindViewById<TextView>(Resource.Id.Text5).Text = item.codEstado;
            view.FindViewById<TextView>(Resource.Id.Text6).Text = item.codDesenlace;
            view.FindViewById<TextView>(Resource.Id.Text7).Text = item.codInstitucion;

            return view;
        }

    }
}