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
    public class TeamEventsController : Controller
    {
        private MgaEntities db = new MgaEntities();

        // GET: TeamEvents
        public ActionResult Index()
        {
            var teamEvents = db.TeamEvents.Include(t => t.Season);
            return View(teamEvents.ToList());
        }

        // GET: TeamEvents/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamEvent teamEvent = db.TeamEvents.Find(id);
            if (teamEvent == null)
            {
                return HttpNotFound();
            }
            return View(teamEvent);
        }

        // GET: TeamEvents/Create
        public ActionResult Create()
        {
            ViewBag.SeasonID = new SelectList(db.Seasons, "SeasonID", "Season1");
            return View();
        }

        // POST: TeamEvents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TeamEventsID,EventName,EventDate,EventType,EventPriority,SeasonID")] TeamEvent teamEvent)
        {
            if (ModelState.IsValid)
            {
                teamEvent.TeamEventsID = Guid.NewGuid();
                db.TeamEvents.Add(teamEvent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SeasonID = new SelectList(db.Seasons, "SeasonID", "Season1", teamEvent.SeasonID);
            return View(teamEvent);
        }

        // GET: TeamEvents/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamEvent teamEvent = db.TeamEvents.Find(id);
            if (teamEvent == null)
            {
                return HttpNotFound();
            }
            ViewBag.SeasonID = new SelectList(db.Seasons, "SeasonID", "Season1", teamEvent.SeasonID);
            return View(teamEvent);
        }

        // POST: TeamEvents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TeamEventsID,EventName,EventDate,EventType,EventPriority,SeasonID")] TeamEvent teamEvent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teamEvent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SeasonID = new SelectList(db.Seasons, "SeasonID", "Season1", teamEvent.SeasonID);
            return View(teamEvent);
        }

        // GET: TeamEvents/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamEvent teamEvent = db.TeamEvents.Find(id);
            if (teamEvent == null)
            {
                return HttpNotFound();
            }
            return View(teamEvent);
        }

        // POST: TeamEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            TeamEvent teamEvent = db.TeamEvents.Find(id);
            db.TeamEvents.Remove(teamEvent);
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
