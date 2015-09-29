using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MGAsite.Models;

namespace MGAsite.Controllers
{
    public class RiderController : Controller
    {
        private MgaEntities db = new MgaEntities();

        // GET: Rider
        public ActionResult Index()
        {
            return View(db.Riders.OrderBy(r=>r.FullName).ToList());
        }

        // GET: Rider/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rider rider = db.Riders.Find(id);
            if (rider == null)
            {
                return HttpNotFound();
            }
            return View(rider);
        }

        // GET: Rider/Create
        public ActionResult Create()
        {
            ViewBag.NationalityId = new SelectList(db.Nationalities, "Id", "Name"); //, rider.NationalityId);
            return View();
        }

        // POST: Rider/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FullName,DOB,NationalityId")] Rider rider)
        {
            if (ModelState.IsValid)
            {
                db.Riders.Add(rider);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rider);
        }

        // GET: Rider/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rider rider = db.Riders.Find(id);
            if (rider == null)
            {
                return HttpNotFound();
            }
            ViewBag.NationalityId = new SelectList(db.Nationalities, "Id", "Name", rider.NationalityId);
            return View(rider);
        }

        // POST: Rider/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FullName,DOB,NationalityId")] Rider rider)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rider).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rider);
        }

        // GET: Rider/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rider rider = db.Riders.Find(id);
            if (rider == null)
            {
                return HttpNotFound();
            }
            return View(rider);
        }

        // POST: Rider/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rider rider = db.Riders.Find(id);
            db.Riders.Remove(rider);
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
