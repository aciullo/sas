using System;
using System.Collections.Generic;

namespace sas.Core
{
    public class SasDatosItem
    {
        public SasDatosItem()
        {
        }
        public int ID { get; set; }
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public string idtabla { get; set; }

        public static implicit operator List<object>(SasDatosItem v)
        {
            throw new NotImplementedException();
        }
    }

}
