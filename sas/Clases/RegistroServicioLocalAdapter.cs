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
    class RegistroServicioLocalAdapter : BaseAdapter <ServicioLocal>
    {
        IList<ServicioLocal> items;
        Activity context;
        public RegistroServicioLocalAdapter(Activity context, IList<ServicioLocal> items)
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
            view.FindViewById<TextView>(Resource.Id.Text2).Text = "Paciente  " + item.nombrePaciente;
            view.FindViewById<TextView>(Resource.Id.Text3).Text = "Cod Estado  " + item.codEstado;
            view.FindViewById<TextView>(Resource.Id.Text4).Text = "Hora Estado " + item.HoraEstado;
            view.FindViewById<TextView>(Resource.Id.Text5).Text = "Estado "  + item.Estado;
            view.FindViewById<TextView>(Resource.Id.Text6).Text = "Desenlace  " + item.codDesenlace;
            view.FindViewById<TextView>(Resource.Id.Text7).Text = "Intitución " + item.codInstitucion;
        

            return view;
        }

    }
}