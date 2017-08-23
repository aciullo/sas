using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using sas_Futura.Models;
using sas_Futura.Controllers.Services;
namespace sas_Futura.Controllers.API
{
    [Authorize]
    public class ABMServiciosApiController : ApiController
    {
        private JsonRepository jsonRepository;


        public ABMServiciosApiController()
        {
            this.jsonRepository = new JsonRepository();
        }
        
        // api/ABMServiciosApi/hola/otro
        public string Get(int idsolicitud, int nrosolicitud, string destino)
        {
            return jsonRepository.Put(idsolicitud, nrosolicitud, destino);
        }


        public string Get(int idsolicitud, int nrosolicitud, string destino, string desenlace)
        {
            return jsonRepository.Put(idsolicitud, nrosolicitud, destino, desenlace);
        }


    }
}