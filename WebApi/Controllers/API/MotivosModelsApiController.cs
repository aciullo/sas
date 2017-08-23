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
    [Authorize]
    public class MotivosModelsApiController : ApiController
    {
        private MotivosModelContext db = new MotivosModelContext();

        // GET: api/MotivosModelsApi
        public IQueryable<MotivosModel> GetMotivosModel()
        {
            return db.MotivosModel.Where(m => m.IdEmpresa == Startup.IdEmpresa);
        }

        // GET: api/MotivosModelsApi/5
        [ResponseType(typeof(MotivosModel))]
        public IHttpActionResult GetMotivosModel(string id)
        {
            MotivosModel motivosModel = db.MotivosModel.Find(id);
            if (motivosModel == null)
            {
                return NotFound();
            }

            return Ok(motivosModel);
        }

        // PUT: api/MotivosModelsApi/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMotivosModel(string id, MotivosModel motivosModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != motivosModel.codMotivo)
            {
                return BadRequest();
            }

            db.Entry(motivosModel).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MotivosModelExists(id))
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

        // POST: api/MotivosModelsApi
        [ResponseType(typeof(MotivosModel))]
        public IHttpActionResult PostMotivosModel(MotivosModel motivosModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.MotivosModel.Add(motivosModel);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (MotivosModelExists(motivosModel.codMotivo))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = motivosModel.codMotivo }, motivosModel);
        }

        // DELETE: api/MotivosModelsApi/5
        [ResponseType(typeof(MotivosModel))]
        public IHttpActionResult DeleteMotivosModel(string id)
        {
            MotivosModel motivosModel = db.MotivosModel.Find(id);
            if (motivosModel == null)
            {
                return NotFound();
            }

            db.MotivosModel.Remove(motivosModel);
            db.SaveChanges();

            return Ok(motivosModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MotivosModelExists(string id)
        {
            return db.MotivosModel.Count(e => e.codMotivo == id) > 0;
        }
    }
}