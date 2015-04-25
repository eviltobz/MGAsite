﻿using System;
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
    public class EventController : Controller
    {
        private MgaEntities db = new MgaEntities();

        // GET: Event
        public ActionResult Index()
        {
            var events = db.Events.Include(x => x.Season);
            return View(events.ToList());
        }

        // GET: Event/Details/5
        public ActionResult Participants(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventParticipantsViewModel model = new EventParticipantsViewModel();
            model.Event = db.Events.Find(id);
            if (model.Event == null)
            {
                return HttpNotFound();
            }
            var teamEntries = db.EventTeamEntries.Where(x => x.EventId == model.Event.Id);
            model.ParticipatingTeams = teamEntries.ToList();
            //model.ParticipatingTeams = participatingTeamEntries.Select(pt => pt.Team).ToList();
            model.OtherTeams = db.Teams.Where(t=>!teamEntries.Any(pt => pt.TeamId == t.Id)).ToList();
            //model.ParticipatingTeams = db.Teams.ToList();
            //model.OtherTeams = db.Teams.ToList();
            return View(model);
        }


        // GET: EventTeamEntry/Add
        public ActionResult AddTeam(int eventId, int teamId)
        {
            var model = new ParticipatingTeam();
            model.Event = db.Events.Find(eventId);
            model.Team = db.Teams.Find(teamId);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTeam([Bind(Include = "Id,TeamId,EventId,Rider1Id,Pony1Id,Rider2Id,Pony2Id,Rider3Id,Pony3Id,Rider4Id,Pony4Id,Rider5Id,Pony5Id")] ParticipatingTeam model)
        {
            if (ModelState.IsValid)
            {
                var teamEntry = new EventTeamEntry() { EventId = model.EventId, TeamId = model.TeamId };
                db.EventTeamEntries.Add(teamEntry);
                db.SaveChanges();
                var r1 = new EventRiderEntry() { EventTeamEntry = teamEntry, RiderId = model.Rider1Id.Value, PonyId = model.Pony1Id.Value };
                var r2 = new EventRiderEntry() { EventTeamEntry = teamEntry, RiderId = model.Rider2Id.Value, PonyId = model.Pony2Id.Value };
                var r3 = new EventRiderEntry() { EventTeamEntry = teamEntry, RiderId = model.Rider3Id.Value, PonyId = model.Pony3Id.Value };
                var r4 = new EventRiderEntry() { EventTeamEntry = teamEntry, RiderId = model.Rider4Id.Value, PonyId = model.Pony4Id.Value };
                var r5 = new EventRiderEntry() { EventTeamEntry = teamEntry, RiderId = model.Rider5Id.Value, PonyId = model.Pony5Id.Value };
                db.EventRiderEntries.AddRange(new EventRiderEntry[] { r1, r2, r3, r4, r5 });
                db.SaveChanges();
                return RedirectToAction("Participants", new { id = model.EventId });
            }

            //ViewBag.TeamId = new SelectList(db.Teams, "Id", "TeamName", model.TeamId);
            //ViewBag.EventId = new SelectList(db.Events, "Id", "EventName", model.EventId);
            return View(model);
        }


        // GET: Event/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Event/Create
        public ActionResult Create()
        {
            ViewBag.SeasonID = new SelectList(db.Seasons, "Id", "SeasonName");
            return View();
        }

        // POST: Event/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EventName,EventDate,EventType,EventPriority,SeasonID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SeasonID = new SelectList(db.Seasons, "Id", "SeasonName", @event.SeasonID);
            return View(@event);
        }

        // GET: Event/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            ViewBag.SeasonID = new SelectList(db.Seasons, "Id", "SeasonName", @event.SeasonID);
            return View(@event);
        }

        // POST: Event/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EventName,EventDate,EventType,EventPriority,SeasonID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SeasonID = new SelectList(db.Seasons, "Id", "SeasonName", @event.SeasonID);
            return View(@event);
        }

        // GET: Event/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            db.Events.Remove(@event);
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


        public ActionResult DeleteParticipant(int? id)
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
        [HttpPost, ActionName("DeleteParticipant")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteParticipantConfirmed(int id)
        {
            EventTeamEntry eventTeamEntry = db.EventTeamEntries.Find(id);
            var eventId = eventTeamEntry.EventId;
            db.EventRiderEntries.RemoveRange(eventTeamEntry.EventRiderEntries);
            db.EventTeamEntries.Remove(eventTeamEntry);
            db.SaveChanges();
            return RedirectToAction("Participants", new { id = eventId });
        }


    }
}