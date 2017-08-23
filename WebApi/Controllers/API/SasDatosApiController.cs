using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using sas_Futura.Controllers.Services;
using sas_Futura.Models;
using System.Web.Http.Description;
using Newtonsoft.Json;
namespace sas_Futura.Controllers.API
{
    [Authorize]
    public class SasDatosApiController : ApiController
    {
         private JsonRepository jsonRepository;
         public SasDatosApiController()
        {
            this.jsonRepository = new JsonRepository();
        }
          [ResponseType(typeof(sasDatosModel))]
         // api/SasDatosApi/hola
     
          public IHttpActionResult Get(string idtabla)
          {
             // var jsonResult = JsonConvert.SerializeObject(jsonRepository.GetAllSasDatos(idtabla));
              var jsonResult = jsonRepository.GetAllSasDatos(idtabla,null);
              
              if (jsonResult != null)
              {
                  return Ok(jsonResult);
              }
              return NotFound();
          }

          public IHttpActionResult Get(string idtabla, string codigo)
          {
              // var jsonResult = JsonConvert.SerializeObject(jsonRepository.GetAllSasDatos(idtabla));
              var jsonResult = jsonRepository.GetAllSasDatos(idtabla, codigo);

              if (jsonResult != null)
              {
                  return Ok(jsonResult);
              }
              return NotFound();
          }
    }
}
