using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using sas_Futura.Models;
using sas_Futura.Class;

namespace sas_Futura.Controllers.UI
{
    public class DeviceUsersController : Controller
    {
        private DeviceUserContext db = new DeviceUserContext();
        private clsAutenticacion encrip = new clsAutenticacion();
        // GET: DeviceUsers
        public ActionResult Index()
        {
            return View(db.DeviceUser.ToList());
        }

        // GET: DeviceUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceUser deviceUser = db.DeviceUser.Find(id);
            if (deviceUser == null)
            {
                return HttpNotFound();
            }
            return View(deviceUser);
        }

        // GET: DeviceUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DeviceUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "usuario,pass,codMovil,nombres,apellidos,idRegistro")] DeviceUser deviceUser)
        {
            if (ModelState.IsValid)
            {
                deviceUser.pass = encrip.Encripta(deviceUser.pass);
                db.DeviceUser.Add(deviceUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(deviceUser);
        }

        // GET: DeviceUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceUser deviceUser = db.DeviceUser.Find(id);
            deviceUser.pass = encrip.Desencripta(deviceUser.pass);
            if (deviceUser == null)
            {
                return HttpNotFound();
            }
            return View(deviceUser);
        }

        // POST: DeviceUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "usuario,pass,codMovil,nombres,apellidos,idRegistro")] DeviceUser deviceUser)
        {
            if (ModelState.IsValid)
            {
                deviceUser.pass = encrip.Encripta(deviceUser.pass);
                db.Entry(deviceUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(deviceUser);
        }

        // GET: DeviceUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceUser deviceUser = db.DeviceUser.Find(id);
            if (deviceUser == null)
            {
                return HttpNotFound();
            }
            return View(deviceUser);
        }

        // POST: DeviceUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DeviceUser deviceUser = db.DeviceUser.Find(id);
            db.DeviceUser.Remove(deviceUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
