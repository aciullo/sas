using System;


namespace Sas.Shared
{
    public class ServicioItem
    {
        public ServicioItem()
        {
        }


        public int IdServicio { get; set; }
        public  string Nombre { get; set; }

        public string Direccion { get; set; }

        public string Motivo { get; set; }

        public string Tipo { get; set; }

        public DateTime Fecha { get; set; }

        public string Estado { get; set; }

        public string IdEstado { get; set; }

        public String HoraEstado { get; set; }

        public string NombreEstado { get; set; }

        public int NroServicio { get; set; }
        public bool Enviado { get; set; }

        public String Detalle
        {
            get
            { 
                return NroServicio.ToString() + ":" + Nombre;
            }
        }
    }

}
