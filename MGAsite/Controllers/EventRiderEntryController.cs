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
    public class EventRiderEntryController : Controller
    {
        private MgaEntities db = new MgaEntities();

        // GET: EventRiderEntry
        public ActionResult Index()
        {
            var eventRiderEntries = db.EventRiderEntries.Include(e => e.Pony).Include(e => e.Rider).Include(e => e.EventTeamEntry);
            return View(eventRiderEntries.ToList());
        }

        // GET: EventRiderEntry/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventRiderEntry eventRiderEntry = db.EventRiderEntries.Find(id);
            if (eventRiderEntry == null)
            {
                return HttpNotFound();
            }
            return View(eventRiderEntry);
        }

        // GET: EventRiderEntry/Create
        public ActionResult Create()
        {
            ViewBag.PonyId = new SelectList(db.Ponies, "Id", "PassportID");
            ViewBag.RiderId = new SelectList(db.Riders, "Id", "FullName");
            var teamEntries = db.EventTeamEntries.Select(x => new { Id = x.Id, Label = x.Event.EventName + "/" + x.Team.TeamName });
            ViewBag.EventTeamEntryId = new SelectList(teamEntries, "Id", "Label");
            //ViewBag.EventTeamEntryId = new SelectList(db.EventTeamEntries, "Id", "Id");
            return View();
        }

        // POST: EventRiderEntry/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EventTeamEntryId,RiderId,PonyId,Paticipated")] EventRiderEntry eventRiderEntry)
        {
            if (ModelState.IsValid)
            {
                db.EventRiderEntries.Add(eventRiderEntry);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PonyId = new SelectList(db.Ponies, "Id", "PassportID", eventRiderEntry.PonyId);
            ViewBag.RiderId = new SelectList(db.Riders, "Id", "FullName", eventRiderEntry.RiderId);
            ViewBag.EventTeamEntryId = new SelectList(db.EventTeamEntries, "Id", "Id", eventRiderEntry.EventTeamEntryId);
            return View(eventRiderEntry);
        }

        // GET: EventRiderEntry/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventRiderEntry eventRiderEntry = db.EventRiderEntries.Find(id);
            if (eventRiderEntry == null)
            {
                return HttpNotFound();
            }
            ViewBag.PonyId = new SelectList(db.Ponies, "Id", "PassportID", eventRiderEntry.PonyId);
            ViewBag.RiderId = new SelectList(db.Riders, "Id", "FullName", eventRiderEntry.RiderId);
            ViewBag.EventTeamEntryId = new SelectList(db.EventTeamEntries, "Id", "Id", eventRiderEntry.EventTeamEntryId);
            return View(eventRiderEntry);
        }

        // POST: EventRiderEntry/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EventTeamEntryId,RiderId,PonyId,Paticipated")] EventRiderEntry eventRiderEntry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventRiderEntry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PonyId = new SelectList(db.Ponies, "Id", "PassportID", eventRiderEntry.PonyId);
            ViewBag.RiderId = new SelectList(db.Riders, "Id", "FullName", eventRiderEntry.RiderId);
            ViewBag.EventTeamEntryId = new SelectList(db.EventTeamEntries, "Id", "Id", eventRiderEntry.EventTeamEntryId);
            return View(eventRiderEntry);
        }

        // GET: EventRiderEntry/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventRiderEntry eventRiderEntry = db.EventRiderEntries.Find(id);
            if (eventRiderEntry == null)
            {
                return HttpNotFound();
            }
            return View(eventRiderEntry);
        }

        // POST: EventRiderEntry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventRiderEntry eventRiderEntry = db.EventRiderEntries.Find(id);
            db.EventRiderEntries.Remove(eventRiderEntry);
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
