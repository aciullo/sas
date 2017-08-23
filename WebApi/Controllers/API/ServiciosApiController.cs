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

namespace sas_Futura.Controllers
{
     [Authorize]
    public class sas_ServiciosApiController : ApiController
    {
        //private sas_Servicios db = new sas_Servicios();
        private ServiciosContext db = new ServiciosContext();
        private JsonRepository jsonRepository;

       
        public sas_ServiciosApiController()
        {
            this.jsonRepository = new JsonRepository();
        }

        // GET: api/sas_ServiciosApi
        public IQueryable<ServiciosModel> Getsas_Servicios()
        {
            return db.sas_servicios.Where(m => m.idEmpresa == Startup.IdEmpresa);

        }

        // GET: api/sas_ServiciosApi/5
        [ResponseType(typeof(ServiciosModel))]
        public IHttpActionResult Getsas_Servicios(int id)
        {
            var sas_servicios = db.sas_servicios.Find(id);
            if (sas_servicios == null)  
            {
                return NotFound();
            }

            return Ok(sas_servicios);
        }

         [ResponseType(typeof(ServiciosModel))]
        public IHttpActionResult Getsas_Servicios(string id, string idestado, string estado)
        {

            var servicios = db.sas_servicios.Where(s => s.codMovil == id && s.codEstado == idestado && s.Estado == estado && s.idEmpresa == Startup.IdEmpresa).ToList();
            if (servicios == null || servicios.Count==0 )
            {
                return NotFound();
            }

            return Ok(servicios);
        }
         [ResponseType(typeof(ServiciosModel))]
         public IHttpActionResult Getsas_ServiciosAll(string id, string idmovil)
         {

             var jsonResult = jsonRepository.GetAllServicios(idmovil);

             if (jsonResult != null)
             {
                 return Ok(jsonResult);
             }
             return NotFound();
         }

         // PUT: api/sas_ServiciosApi/5
         [ResponseType(typeof(void))]
         public IHttpActionResult Putsas_Servicios(int id, ServiciosModel sas_Servicios)
         {
             if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }

             if (id != sas_Servicios.id_Solicitud)
             {
                 return BadRequest();
             }

             //db.Entry(sas_Servicios).State = EntityState.Modified;

             try
             
             {
                 var ser = db.sas_servicios.Where(s => s.id_Solicitud == id & s.idEmpresa == Startup.IdEmpresa).SingleOrDefault();
                 if (ser != null)
                 {
                    
                     ser.SAT = sas_Servicios.SAT;
                     ser.sv_fc= sas_Servicios.sv_fc;
                     ser.sv_fresp = sas_Servicios.sv_fresp;
                     ser.sv_ta = sas_Servicios.sv_ta;
                     ser.sv_tempe = sas_Servicios.sv_tempe;
                     ser.Glasgow = sas_Servicios.Glasgow;
                     ser.Glicemia = sas_Servicios.Glicemia;
                     ser.IndicacionArribo = sas_Servicios.IndicacionArribo;

                     db.Entry(ser).State = EntityState.Modified;
                     db.SaveChanges();
                 }
                 
             }
             catch (DbUpdateConcurrencyException)
             {
                 if (!sas_ServiciosExists(id))
                 {
                     return NotFound();
                 }
                 else
                 {
                     throw;
                 }
             }

             return StatusCode(HttpStatusCode.NoContent);
         }

    //    // POST: api/sas_ServiciosApi
    //    [ResponseType(typeof(sas_Servicios))]
    //    public IHttpActionResult Postsas_Servicios(sas_Servicios sas_Servicios)
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return BadRequest(ModelState);
    //        }

    //        db.sas_Servicios.Add(sas_Servicios);

    //        try
    //        {
    //            db.SaveChanges();
    //        }
    //        catch (DbUpdateException)
    //        {
    //            if (sas_ServiciosExists(sas_Servicios.idSolicitud))
    //            {
    //                return Conflict();
    //            }
    //            else
    //            {
    //                throw;
    //            }
    //        }

    //        return CreatedAtRoute("DefaultApi", new { id = sas_Servicios.idSolicitud }, sas_Servicios);
    //    }

    //    // DELETE: api/sas_ServiciosApi/5
    //    [ResponseType(typeof(sas_Servicios))]
    //    public IHttpActionResult Deletesas_Servicios(int id)
    //    {
    //        sas_Servicios sas_Servicios = db.sas_Servicios2.Find(id);
    //        if (sas_Servicios == null)
    //        {
    //            return NotFound();
    //        }

    //        db.sas_Servicios.Remove(sas_Servicios);
    //        db.SaveChanges();

    //        return Ok(sas_Servicios);
    //    }

         protected override void Dispose(bool disposing)
         {
             if (disposing)
             {
                 db.Dispose();
             }
             base.Dispose(disposing);
         }

         private bool sas_ServiciosExists(int id)
         {
             return db.sas_servicios.Count(e => e.id_Solicitud == id) > 0;
         }
    }
}