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
    public class PonyController : Controller
    {
        private MgaEntities db = new MgaEntities();

        // GET: Pony
        public ActionResult Index()
        {
            return View(db.Ponies.ToList());
        }

        // GET: Pony/Details/5
        public ActionResult Details(int? id)
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

        // GET: Pony/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Pony/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PassportID,Name,DOB")] Pony pony)
        {
            if (ModelState.IsValid)
            {
                db.Ponies.Add(pony);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pony);
        }

        // GET: Pony/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Pony/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PassportID,Name,DOB")] Pony pony)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pony).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pony);
        }

        // GET: Pony/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Pony/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
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
