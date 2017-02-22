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
    class RegistroServicioLocalAdapter : BaseAdapter <ServicioLocalItem>
    {
        IList<ServicioLocalItem> items;
        Activity context;
        public RegistroServicioLocalAdapter(Activity context, IList<ServicioLocalItem> items)
        : base()
         {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }

        public override ServicioLocalItem this[int position]
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

            string estado = "";
            switch (item.codEstado)
            {
                case ("002"):
                    estado = "Pendiente";
                    break;

                case "009":
                    estado = "Concluído";
                    break;
                default:
                    estado = "En Proceso";
                    break;

            }

            View view;

            if (item.codEstado != "008")
            { 
            // TWO LINE LIST ITEM
            view = context.LayoutInflater.Inflate(Android.Resource.Layout.TwoLineListItem, null);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = estado + ", " + item.nombrePaciente;
            view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.direccionReferecia;
            }

            else

            { 

                view = convertView;
                if (view == null) // no view to re-use, create new
                    view = context.LayoutInflater.Inflate(Resource.Layout.ServicioLocalView,parent,false);
                view.FindViewById<TextView>(Resource.Id.Text1).Text = item.ID.ToString();
                view.FindViewById<TextView>(Resource.Id.Text2).Text = "Paciente  " + item.nombrePaciente;
                view.FindViewById<TextView>(Resource.Id.Text3).Text = "Cod Estado  " + item.codEstado;
                view.FindViewById<TextView>(Resource.Id.Text4).Text = "Hora Estado " + item.HoraEstado;
                view.FindViewById<TextView>(Resource.Id.Text5).Text = "Estado "  + item.Estado;
                view.FindViewById<TextView>(Resource.Id.Text6).Text = "Desenlace  " + item.codDesenlace;
                view.FindViewById<TextView>(Resource.Id.Text7).Text = "Intitución " + item.codInstitucion;

                view.FindViewById(Resource.Id.Text).Click += delegate {
                    Toast.MakeText(context, "tab list", ToastLength.Short).Show();
                };

                view.FindViewById(Resource.Id.secondary_action).Click += delegate {
                    Toast.MakeText(context, Resource.String.touched_secondary_message, ToastLength.Short).Show();
                };

                
            }

            //if (item.Estado != "008")
            //{
            //    view.FindViewById(Resource.Id.secondary_action).Visibility = ViewStates.Invisible;

            //}


        




            return view;
        }

    }
}