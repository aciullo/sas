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
using sas_Futura.Class;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace sas_Futura.Controllers.API
{
    public class DeviceUsersApiController : ApiController
    {
        private DeviceUserContext db = new DeviceUserContext();
        private sasProcesoEstadoServicioContext s = new sasProcesoEstadoServicioContext();
        private clsAutenticacion encrip = new clsAutenticacion();
        // GET: api/DeviceUsersApi
        //public IQueryable<DeviceUser> GetDeviceUser()
        //{
        //    return db.DeviceUser;
        //}

        //// GET: api/DeviceUsersApi/5
        //[ResponseType(typeof(DeviceUser))]
        //public IHttpActionResult GetDeviceUser(string id)
        //{
        //    DeviceUser deviceUser = db.DeviceUser.Find(id);
        //    if (deviceUser == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(deviceUser);
        //}
       
        [ResponseType(typeof(DeviceUser))]
        public IHttpActionResult GetDeviceUser(string id, string idestado)
        {
            var deviceUser = db.DeviceUser.Where(u => u.usuario == id && u.pass == idestado).ToList();
            if (deviceUser == null || deviceUser.Count==0)
            {
                return NotFound();
            }

            return Ok(deviceUser);
        }

        [ResponseType(typeof(string))]
        public IHttpActionResult GetDeviceUser(string id)
        {
            
            var deviceUser = db.DeviceUser.Where(u => u.codMovil==id).ToList();
            //para el log
            var vsasProcesoEstadoServicio = new sasProcesoEstadoServicio() { Nombre = "Envio Solicitud del movil " + id, IdEmpresa = Startup.IdEmpresa };

            if (deviceUser == null || deviceUser.Count == 0)
            {
                return NotFound();
            }

             string MESSAGE = "Nuevo Servicio ";
             string resultado="";

            foreach (var user in deviceUser)
            {
                vsasProcesoEstadoServicio.Nombre = vsasProcesoEstadoServicio.Nombre + " " + user.nombres + " " + user.apellidos;

                var jGcmData = new JObject();
                var jData = new JObject();

                jData.Add("message", MESSAGE);
               // jGcmData.Add("to", "/topics/global");
               jGcmData.Add("to", user.idRegistro);
                //jGcmData.Add("to", user.idRegistro + "/topics/global");

                jGcmData.Add("data", jData);

                var url = new Uri("https://gcm-http.googleapis.com/gcm/send");
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));

                        client.DefaultRequestHeaders.TryAddWithoutValidation(
                            "Authorization", "key=" + Startup.API_KEY);

                        Task.WaitAll(client.PostAsync(url,
                            new StringContent(jGcmData.ToString(), Encoding.Default, "application/json"))
                                .ContinueWith(response =>
                                {
                                    resultado= response.Status.ToString();
                                    resultado= resultado + " " +  response.Result.ToString();
                                    resultado = resultado + " " + "Message sent: check the client device notification tray.";
                                    vsasProcesoEstadoServicio.codEstado = response.Result.StatusCode.ToString();
                                }));
                    }
                }
                catch (Exception e)
                {
                    resultado = ("Unable to send GCM message:");
                    resultado = resultado + " STRACKTRACE:  " + (e.StackTrace);
                }

                s.sasProcesoEstadoServicio.Add(vsasProcesoEstadoServicio);
                s.SaveChanges();
            }


           
            

            return Ok(resultado);
        }
        //// PUT: api/DeviceUsersApi/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PutDeviceUser(string id, DeviceUser deviceUser)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != deviceUser.usuario)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(deviceUser).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!DeviceUserExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //// POST: api/DeviceUsersApi
        [ResponseType(typeof(DeviceUser))]
        public IHttpActionResult PostDeviceUser(DeviceUser deviceUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            deviceUser.pass = encrip.Encripta(deviceUser.pass);
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

        //// DELETE: api/DeviceUsersApi/5
        //[ResponseType(typeof(DeviceUser))]
        //public IHttpActionResult DeleteDeviceUser(string id)
        //{
        //    DeviceUser deviceUser = db.DeviceUser.Find(id);
        //    if (deviceUser == null)
        //    {
        //        return NotFound();
        //    }

        //    db.DeviceUser.Remove(deviceUser);
        //    db.SaveChanges();

        //    return Ok(deviceUser);
        //}

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