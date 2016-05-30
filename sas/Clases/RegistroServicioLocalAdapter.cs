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

namespace sas.Clases
{
    class RegistroServicioLocalAdapter : BaseAdapter <RegistrarServicioModel>
    {
        List<RegistrarServicioModel> items;
        Activity context;
        public RegistroServicioLocalAdapter(Activity context, List<RegistrarServicioModel> items)
        : base()
         {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }

        public override RegistrarServicioModel this[int position]
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
            view.FindViewById<TextView>(Resource.Id.Text1).Text = item.id_registro.ToString();
            view.FindViewById<TextView>(Resource.Id.Text2).Text = item.id_Solicitud.ToString();
            view.FindViewById<TextView>(Resource.Id.Text3).Text = item.codMovil;
            view.FindViewById<TextView>(Resource.Id.Text4).Text = item.hora_Llegada;
            view.FindViewById<TextView>(Resource.Id.Text5).Text = item.idestado;
            view.FindViewById<TextView>(Resource.Id.Text6).Text = item.idDesenlace;
            view.FindViewById<TextView>(Resource.Id.Text7).Text = item.idInstitucion;




            return view;
        }

    }
}