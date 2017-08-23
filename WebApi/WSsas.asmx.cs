using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using sas_Futura.Controllers.API;
using sas_Futura.Controllers.Services;
using sas_Futura.Models;
using System.Web.Http.Description;
using System.Web.Http;
using Newtonsoft.Json;
namespace sas_Futura
{
    /// <summary>
    /// Summary description for WSsas
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WSsas : System.Web.Services.WebService
    {

        ServiciosContext db = new ServiciosContext();
        private JsonRepository jsonRepository;
        public WSsas()
        {
            this.jsonRepository = new JsonRepository();
        }

        [WebMethod]
        // api/ABMServiciosApi/hola/otro
        public string PutDestino(int idsolicitud, int nrosolicitud, string destino)
        {
            return jsonRepository.Put(idsolicitud, nrosolicitud, destino);
        }
        
        [WebMethod]

        public string PutDesenlace(int idsolicitud, int nrosolicitud, string destino, string desenlace)
        {
            return jsonRepository.Put(idsolicitud, nrosolicitud, destino, desenlace);
        }

 
        
        public IQueryable<ServiciosModel> Getsas_Servicios()
        {
            return db.sas_servicios;

        }
         [WebMethod]
         public string Getsas_Servicios(string id, string idestado, string estado)
        {
          

            var servicios = db.sas_servicios.Where(s => s.codMovil == id && s.codEstado == idestado && s.Estado == estado).ToList();
            //if (servicios == null)
            //{
            //    return NotFound();
            //}

            var jsonResult = JsonConvert.SerializeObject(servicios);


            return jsonResult;

        }
    }
}
