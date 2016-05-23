using System;


namespace sas
{
	public class RegistrarServicioModel
        //esta clase se utiliza para el registro local de las actividades

	{
		
		public int id_registro { get; set; }

		public int id_Solicitud { get; set; }
		public int NumeroSolicitud { get; set; }
		public string codMovil { get; set; }
		public string hora_Llegada { get; set; }
		public string idestado { get; set;}
		//public Nullable<System.DateTime> Audit_Fecha { get; set; }
		public string Audit_Usuario { get; set; }


		public string Audit_Fecha
		{
			get
			{
				return string.Format("{0:yy-MM-dd}", System.DateTime.Now);
			}
		}
	}



}

