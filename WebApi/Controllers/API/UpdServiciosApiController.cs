using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using sas_Futura.Controllers.Services;

namespace sas_Futura.Controllers.API
{
    [Authorize]
    public class UpdServiciosApiController : ApiController
    {

        private JsonRepository jsonRepository;

        // api/UpdServiciosApiController/
        public UpdServiciosApiController()
        {
            this.jsonRepository = new JsonRepository();
        }
        
        // api/UpdServiciosApiController/hola
        public string Get(int idsolicitud , string codestado, string hora)
        {
            return jsonRepository.GetAllJsons(idsolicitud, codestado, hora);
        }
       
        // api/UpdServiciosApiController/hola/otro
        public string GetUpdate(int idsolicitud, int nrosolicitud, string destino, string IdProductoFinal)
        {
            return jsonRepository.Put(idsolicitud, nrosolicitud, destino, IdProductoFinal);
        }
    }
}
