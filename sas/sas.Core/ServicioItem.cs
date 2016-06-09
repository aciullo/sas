using System;


namespace sas.Core
{
    public class ServicioItem
    {
        public ServicioItem()
        {
        }

        public int ID { get; set; }
        public int id_Solicitud { get; set; }
        public int NumeroSolicitud { get; set; }
        public  string Nombre { get; set; }
        public DateTime Fecha { get; set; }
        public string codMovil { get; set; }
        public string Estado { get; set; }
        public string codEstado { get; set; }
        public string HoraEstado { get; set; }
        public string codInstitucion { get; set; }
        public string codDesenlace { get; set; }
        public bool Enviado { get; set; }

        public string AuditUsuario { get; set; }
        public int AuditId { get; set; }
        public string GeoData { get; set; }
        public string Address { get; set; }

        public string Detalle
        {
            get
            { 
                return NumeroSolicitud.ToString() + ":" + Nombre;
            }
        }
    }

}
