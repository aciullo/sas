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
using sas.Core;

namespace sas
{

    public class ServicesAdapter : BaseAdapter<ServicioLocalItem>
    {
        IList<ServicioLocalItem> items;
        Activity context;

        public ServicesAdapter(Activity context, IList<ServicioLocalItem> items)
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
            string estado="";
            // SIMPLE LIST ITEM 1
            //View view = convertView;
            //if (view == null)
            //    view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.nombrePaciente;

            // SIMPLE LIST ITEM 2
            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem2, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.nombrePaciente;
            //view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.direccionReferecia;
            switch (item.codEstado)
            {
                case ("002"):
                    estado = "Pendiente";
                    break;
              
                case "009":
                    estado = "Conclu�do";
                    break;
                default:
                    estado = "En Proceso";
                    break;

            }

            // TWO LINE LIST ITEM
            View view = context.LayoutInflater.Inflate(Android.Resource.Layout.TwoLineListItem, null);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = estado + ", " +  item.nombrePaciente ;
            view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.direccionReferecia;
         
            // SIMPLE e LIST ITEM
            // View view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleSelectableListItem, null);
            // view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;

            // SIMPLE LIST ITEM ACTIVATED 1
            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItemActivated1, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;

            // SIMPLE LIST ITEM ACTIVATED 2
            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItemActivated2, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;
            //view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.SubHeading;

            // SIMPLE LIST ITEM CHECKED
            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItemChecked, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;

            // SIMPLE LIST ITEM MULTIPLE CHOICE
            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItemMultipleChoice, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;

            // SIMPLE LIST ITEM SINGLE CHOICE
            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItemSingleChoice, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;

            // TWO LINE LIST ITEM
            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.TwoLineListItem, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;
            //view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.SubHeading;

            // ACTIVITY LIST ITEM
            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.ActivityListItem, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;
            //view.FindViewById<ImageView>(Android.Resource.Id.Icon).SetImageResource(item.ImageResourceId);

            // TEST LIST ITEM
            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.TestListItem, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;

            return view;
        }
    }
}