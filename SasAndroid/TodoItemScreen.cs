using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Sas.Shared;
using SasAndroid;
using System;

namespace TaskyAndroid.Screens 
{
	/// <summary>
	/// View/edit a Task
	/// </summary>
	[Activity (Label = "Servicio")]			
	public class ServicioItemScreen : Activity 
	{
		ServicioItem task = new ServicioItem();
		Button cancelDeleteButton;
		EditText notesTextEdit;
		EditText nameTextEdit;
		Button saveButton;
		CheckBox doneCheckbox;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			int taskID = Intent.GetIntExtra("TaskID", 0);

			if(taskID > 0) {
				task = ServicioItemManager.GetTask(taskID);
			}
			
			// set our layout to be the home screen
			SetContentView(Resource.Layout.TaskDetails);
			nameTextEdit = FindViewById<EditText>(Resource.Id.NameText);
			notesTextEdit = FindViewById<EditText>(Resource.Id.NotesText);
			saveButton = FindViewById<Button>(Resource.Id.SaveButton);

			// TODO: find the Checkbox control and set the value
			doneCheckbox = FindViewById<CheckBox>(Resource.Id.chkDone);
			doneCheckbox.Checked = task.Enviado;

			// find all our controls
			cancelDeleteButton = FindViewById<Button>(Resource.Id.CancelDeleteButton);
			
			// set the cancel delete based on whether or not it's an existing task
			cancelDeleteButton.Text = (task.IdServicio == 0 ? "Cancel" : "Delete");
			
			nameTextEdit.Text = task.Nombre; 
			notesTextEdit.Text = task.NroServicio.ToString();

			// button clicks 
			cancelDeleteButton.Click += (sender, e) => { CancelDelete(); };
			saveButton.Click += (sender, e) => { Save(); };
		}

		void Save()
		{
			task.Nombre = nameTextEdit.Text;
			task.NroServicio = Convert.ToInt32( notesTextEdit.Text);

			//TODO: 
			task.Enviado = doneCheckbox.Checked;

			ServicioItemManager.SaveTask(task);

            //StartService(new Intent("py.com.futura.SasService"));

            Intent serviceIntent = new Intent("py.com.futura.SasService"); 

            serviceIntent.PutExtra("Tipo", "ES");
   

            StartService(serviceIntent);

            Finish();
		}
		
		void CancelDelete()
		{
			if (task.IdServicio != 0) {
				ServicioItemManager.DeleteTask(task.IdServicio);
			}
			Finish();
		}
	}
}