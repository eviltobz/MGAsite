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
    public class EventTeamEntryController : Controller
    {
        private MgaEntities db = new MgaEntities();

        // GET: EventTeamEntry
        public ActionResult Index()
        {
            var eventTeamEntries = db.EventTeamEntries.Include(e => e.Team).Include(e => e.Event);
            return View(eventTeamEntries.ToList());
        }

        // GET: EventTeamEntry/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventTeamEntry eventTeamEntry = db.EventTeamEntries.Find(id);
            if (eventTeamEntry == null)
            {
                return HttpNotFound();
            }
            return View(eventTeamEntry);
        }

        // GET: EventTeamEntry/Add
        public ActionResult Add(int eventId, int teamId)
        {
            var model = new ParticipatingTeam();
            model.Event = db.Events.Find(eventId);
            model.Team = db.Teams.Find(teamId);
            model.EventType = model.Event.EventType1.Type;

            var participants = model.Event.EventTeamEntries.SelectMany(e => e.EventRiderEntries).ToList();

            //var availableRiders = db.Riders.Where(r => !participants.Any(p => r.Id == p.RiderId)).Select(x=>new {Id = x.Id, Label=x.FullName});
            //var availableRiders = db.Riders.Except(participants.Select(p=>p.Rider)).Select(x=>new {Id = x.Id, Label=x.FullName});
            //var availablePonies = db.Ponies.Where(x => !participants.Any(p => x.Id == p.PonyId)).Select(x => new { Id = x.Id, Label = x.Name });

            //var teamEntries = db.EventTeamEntries.Select(x => new { Id = x.Id, Label = x.Event.EventName + "/" + x.Team.TeamName });
            var availableRiders = db.Riders.Select(x => new { Id = x.Id, Label = x.FullName });
            var availablePonies = db.Ponies.Select(x => new { Id = x.Id, Label = x.Name });
            model.AvailableRiders = new SelectList(availableRiders, "Id", "Label");
            model.AvailablePonies = new SelectList(availablePonies, "Id", "Label");

            //ViewBag.TeamId = new SelectList(db.Teams, "Id", "TeamName");
            //ViewBag.EventId = new SelectList(db.Events, "Id", "EventName");
            return View(model);
        }

        // GET: EventTeamEntry/Create
        public ActionResult Create()
        {
            ViewBag.TeamId = new SelectList(db.Teams, "Id", "TeamName");
            ViewBag.EventId = new SelectList(db.Events, "Id", "EventName");
            return View();
        }

        // POST: EventTeamEntry/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TeamId,EventId,Points")] EventTeamEntry eventTeamEntry)
        {
            if (ModelState.IsValid)
            {
                db.EventTeamEntries.Add(eventTeamEntry);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TeamId = new SelectList(db.Teams, "Id", "TeamName", eventTeamEntry.TeamId);
            ViewBag.EventId = new SelectList(db.Events, "Id", "EventName", eventTeamEntry.EventId);
            return View(eventTeamEntry);
        }

        // GET: EventTeamEntry/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventTeamEntry eventTeamEntry = db.EventTeamEntries.Find(id);
            if (eventTeamEntry == null)
            {
                return HttpNotFound();
            }
            ViewBag.TeamId = new SelectList(db.Teams, "Id", "TeamName", eventTeamEntry.TeamId);
            ViewBag.EventId = new SelectList(db.Events, "Id", "EventName", eventTeamEntry.EventId);
            return View(eventTeamEntry);
        }

        // POST: EventTeamEntry/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TeamId,EventId,Points")] EventTeamEntry eventTeamEntry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventTeamEntry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TeamId = new SelectList(db.Teams, "Id", "TeamName", eventTeamEntry.TeamId);
            ViewBag.EventId = new SelectList(db.Events, "Id", "EventName", eventTeamEntry.EventId);
            return View(eventTeamEntry);
        }

        // GET: EventTeamEntry/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventTeamEntry eventTeamEntry = db.EventTeamEntries.Find(id);
            if (eventTeamEntry == null)
            {
                return HttpNotFound();
            }
            return View(eventTeamEntry);
        }

        // POST: EventTeamEntry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventTeamEntry eventTeamEntry = db.EventTeamEntries.Find(id);
            db.EventTeamEntries.Remove(eventTeamEntry);
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
