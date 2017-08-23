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
    public class UsersApiController : ApiController
    {
        private DeviceUserContext db = new DeviceUserContext();

        // GET: api/UsersApi
        public IQueryable<DeviceUser> GetDeviceUser()
        {
            return db.DeviceUser;
        }

        // GET: api/UsersApi/5
        [ResponseType(typeof(DeviceUser))]
        public IHttpActionResult GetDeviceUser(string id)
        {
            DeviceUser deviceUser = db.DeviceUser.Find(id);
            if (deviceUser == null)
            {
                return NotFound();
            }

            return Ok(deviceUser);
        }

        // GET: api/UsersApi/usuario/pass
        [ResponseType(typeof(DeviceUser))]
        public IHttpActionResult GetDeviceUser(string id, string idestado)
        {
            var deviceUser = db.DeviceUser.Where(u => u.usuario == id && u.pass == idestado).ToList();
            if (deviceUser == null || deviceUser.Count == 0)
            {
                return NotFound();
            }

            return Ok(deviceUser);
        }

        // PUT: api/UsersApi/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDeviceUser(string id, DeviceUser deviceUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != deviceUser.usuario)
            {
                return BadRequest();
            }

          //  db.Entry(deviceUser).State = EntityState.Modified;
            
            
            try
            {
               // db.SaveChanges();

                var usuario = db.DeviceUser.Where(u => u.usuario == id && u.codMovil.Contains(deviceUser.codMovil)).SingleOrDefault();
                if (usuario!=null)
                {
                    usuario.idRegistro = deviceUser.idRegistro;
                    db.Entry(usuario).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceUserExists(id))
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

        // POST: api/UsersApi
        [ResponseType(typeof(DeviceUser))]
        public IHttpActionResult PostDeviceUser(DeviceUser deviceUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DeviceUser.Add(deviceUser);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (DeviceUserExists(deviceUser.usuario))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = deviceUser.usuario }, deviceUser);
        }

        // DELETE: api/UsersApi/5
        [ResponseType(typeof(DeviceUser))]
        public IHttpActionResult DeleteDeviceUser(string id)
        {
            DeviceUser deviceUser = db.DeviceUser.Find(id);
            if (deviceUser == null)
            {
                return NotFound();
            }

            db.DeviceUser.Remove(deviceUser);
            db.SaveChanges();

            return Ok(deviceUser);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DeviceUserExists(string id)
        {
            return db.DeviceUser.Count(e => e.usuario == id) > 0;
        }
    }
}