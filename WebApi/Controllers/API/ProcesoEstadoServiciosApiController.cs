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

namespace sas_Futura.Controllers.API
{
    public class ProcesoEstadoServiciosApiController : ApiController
    {
        private sasProcesoEstadoServicioContext db = new sasProcesoEstadoServicioContext();

        // GET: api/ProcesoEstadoServiciosApi
        public IQueryable<sasProcesoEstadoServicio> GetsasProcesoEstadoServicio()
        {
            return db.sasProcesoEstadoServicio;
        }

        // GET: api/ProcesoEstadoServiciosApi/5
        [ResponseType(typeof(sasProcesoEstadoServicio))]
        public IHttpActionResult GetsasProcesoEstadoServicio(int id)
        {
            sasProcesoEstadoServicio sasProcesoEstadoServicio = db.sasProcesoEstadoServicio.Find(id);
            if (sasProcesoEstadoServicio == null)
            {
                return NotFound();
            }

            return Ok(sasProcesoEstadoServicio);
        }

        // PUT: api/ProcesoEstadoServiciosApi/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutsasProcesoEstadoServicio(int id, sasProcesoEstadoServicio sasProcesoEstadoServicio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sasProcesoEstadoServicio.idLog)
            {
                return BadRequest();
            }

            db.Entry(sasProcesoEstadoServicio).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!sasProcesoEstadoServicioExists(id))
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

        // POST: api/ProcesoEstadoServiciosApi
        [ResponseType(typeof(sasProcesoEstadoServicio))]
        public IHttpActionResult PostsasProcesoEstadoServicio(sasProcesoEstadoServicio sasProcesoEstadoServicio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            sasProcesoEstadoServicio.IdEmpresa = Startup.IdEmpresa;
            db.sasProcesoEstadoServicio.Add(sasProcesoEstadoServicio);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = sasProcesoEstadoServicio.idLog }, sasProcesoEstadoServicio);
        }

        // DELETE: api/ProcesoEstadoServiciosApi/5
        [ResponseType(typeof(sasProcesoEstadoServicio))]
        public IHttpActionResult DeletesasProcesoEstadoServicio(int id)
        {
            sasProcesoEstadoServicio sasProcesoEstadoServicio = db.sasProcesoEstadoServicio.Find(id);
            if (sasProcesoEstadoServicio == null)
            {
                return NotFound();
            }

            db.sasProcesoEstadoServicio.Remove(sasProcesoEstadoServicio);
            db.SaveChanges();

            return Ok(sasProcesoEstadoServicio);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool sasProcesoEstadoServicioExists(int id)
        {
            return db.sasProcesoEstadoServicio.Count(e => e.idLog == id) > 0;
        }
    }
}