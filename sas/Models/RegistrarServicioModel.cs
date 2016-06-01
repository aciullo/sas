using SQLite.Net.Attributes;
using System;


namespace sas
{
	public class RegistrarServicioModel
        //esta clase se utiliza para el registro local de las actividades

	{
        [PrimaryKey, AutoIncrement]
        public int id_registro { get; set; }

		public int id_Solicitud { get; set; }
		public int NumeroSolicitud { get; set; }
		public string codMovil { get; set; }
		public string HoraEstado { get; set; }
		public string codEstado { get; set;}
        public string Estado { get; set; }
        //public Nullable<System.DateTime> Audit_Fecha { get; set; }
        public string codInstitucion { get; set; }
        public string codDesenlace { get; set; }
        public bool sincronizado { get; set; }


        public string Audit_Fecha
		{
			get
			{
				return string.Format("{0:yy-MM-dd}", System.DateTime.Now);
			}
		}
	}



}

