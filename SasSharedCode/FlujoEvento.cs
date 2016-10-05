using System;
using System.Collections.Generic;
using System.Text;

namespace Sas.Shared
{
    public class FlujoEvento
    {
        public int IdFlujo { get; set; }
        public int IdServicio { get; set; }

        public DateTime Fecha { get; set; }

        public String Estado { get; set; }

        public String IdEstado { get; set; }

        public bool Enviado { get; set; }

        public string AuditUsuario { get; set; }

        public int AuditId { get; set; }

        public String GeoData { get; set; }


    }
}
