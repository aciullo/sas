using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace sas_Futura.Models
{
    public class sasDatosModel
    {
          
            [Key]
            public string codigo { get; set; }
         
            public string descripcion { get; set; }
            public string idtabla { get; set; }
        
    }
}