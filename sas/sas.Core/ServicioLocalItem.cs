using System;

namespace sas.Core {
	/// <summary>
	/// Task business object
	/// </summary>
	public class ServicioLocalItem
    {
		public ServicioLocalItem()
		{
		}

        public int ID { get; set; }
        public int id_Solicitud { get; set; }
        public int NumeroSolicitud { get; set; }
        public DateTime fecha_Llamado { get; set; }
        public string hora_Llamado { get; set; }
        public string nombrePaciente { get; set; }
        public string Tel { get; set; }
        public decimal edadPaciente { get; set; }
        public string nombreSolicitante { get; set; }
        public string direccionReferecia { get; set; }
        public string direccionReferecia2 { get; set; }
        public string numeroCasa { get; set; }
        public string referencia { get; set; }
        public string Motivo { get; set; }
        public string nroSalida { get; set; }
        public string codMovil { get; set; }
        public string codChofer { get; set; }
        public string codParamedico { get; set; }
        public string codMedico { get; set; }
        public string Acompañante { get; set; }
        public string observacion { get; set; }
        public string Estado { get; set; }
        public string codEstado { get; set; }
        public string HoraEstado { get; set; }
        public string codMotivo1 { get; set; }
        public string codMotivo2 { get; set; }
        public string codMotivo3 { get; set; }
        public string OtroMotivo { get; set; }
        public string codTipo { get; set; }
        public string codInstitucion { get; set; }
        public string codDesenlace { get; set; }
        public string producto { get; set; }
    }
}