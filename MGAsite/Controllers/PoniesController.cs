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
        [Authorize]
    public class PoniesController : Controller
    {
        private MgaEntities db = new MgaEntities();

        // GET: Ponies
        public ActionResult Index()
        {
            return View(db.Ponies.ToList());
        }

        // GET: Ponies/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pony pony = db.Ponies.Find(id);
            if (pony == null)
            {
                return HttpNotFound();
            }
            return View(pony);
        }

        // GET: Ponies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ponies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PoniesID,PassportID,PonyName,DOB")] Pony pony)
        {
            if (ModelState.IsValid)
            {
                pony.PoniesID = Guid.NewGuid();
                db.Ponies.Add(pony);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pony);
        }

        // GET: Ponies/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pony pony = db.Ponies.Find(id);
            if (pony == null)
            {
                return HttpNotFound();
            }
            return View(pony);
        }

        // POST: Ponies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PoniesID,PassportID,PonyName,DOB")] Pony pony)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pony).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pony);
        }

        // GET: Ponies/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pony pony = db.Ponies.Find(id);
            if (pony == null)
            {
                return HttpNotFound();
            }
            return View(pony);
        }

        // POST: Ponies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Pony pony = db.Ponies.Find(id);
            db.Ponies.Remove(pony);
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
