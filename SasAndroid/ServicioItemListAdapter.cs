using System.Collections.Generic;
using Android.App;
using Android.Widget;
using Sas.Shared;

namespace SasAndroid.ApplicationLayer 
{
	/// <summary>
	/// Adapter that presents Tasks in a row-view
	/// </summary>
	public class ServicioItemListAdapter : BaseAdapter<ServicioItem> 
	{
		Activity context = null;
		IList<ServicioItem> servicios = new List<ServicioItem>();
		
		public ServicioItemListAdapter(Activity context, IList<ServicioItem> tasks) : base ()
		{
			this.context = context;
			this.servicios = tasks;
		}
		
		public override ServicioItem this[int position]
		{
			get { return servicios[position]; }
		}
		
		public override long GetItemId (int position)
		{
			return position;
		}
		
		public override int Count
		{
			get { return servicios.Count; }
		}
		
		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			// Get our object for position
			var item = servicios[position];

            //Try to reuse convertView if it's not  null, otherwise inflate it from our item layout
            // gives us some performance gains by not always inflating a new view
            // will sound familiar to MonoTouch developers with UITableViewCell.DequeueReusableCell()

            var view = (convertView ??
                    context.LayoutInflater.Inflate(
                    Resource.Layout.TaskListItem,
                    parent,
                    false)) as LinearLayout;
            // Find references to each subview in the list item's view
            var txtName = view.FindViewById<TextView>(Resource.Id.NameText);
            var txtDescription = view.FindViewById<TextView>(Resource.Id.NotesText);
            //Assign item's values to the various subviews
            txtName.SetText(item.Detalle, TextView.BufferType.Normal);
            txtDescription.SetText(item.NroServicio.ToString(), TextView.BufferType.Normal);

   //         // TODO: use this code to populate the row, and remove the above view
   //         var view = (convertView ??
			//	context.LayoutInflater.Inflate(
			//		Android.Resource.Layout.SimpleListItemChecked,
			//		parent,
			//		false)) as CheckedTextView;
			//view.SetText (item.Nombre==""?"<new task>":item.Detalle, TextView.BufferType.Spannable);
			//view.Checked = item.Enviado;


			//Finally return the view
			return view;
		}
	}
}