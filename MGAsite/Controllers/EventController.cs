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
            model.EventType = model.Event.EventType1.Type;

            var teamEntries = db.EventTeamEntries.Where(x => x.EventId == model.Event.Id).OrderBy(t=>t.Team.TeamName).ThenBy(t=>t.Under17s);
            model.ParticipatingTeams = teamEntries.ToList();
            model.OtherTeams = new List<Tuple<Team, bool, bool>>();
            //var otherTeams = db.Teams.ToList().Select(t => new {team = t, open = true, under17s = true}); //.ToList();
            foreach (var team in db.Teams.OrderBy(t=>t.TeamName).ToList())
            {
                var under17s = !model.ParticipatingTeams.Any(pt => pt.TeamId == team.Id && pt.Under17s);
                var open = !model.ParticipatingTeams.Any(pt => pt.TeamId == team.Id && !pt.Under17s);

                if (under17s || open)
                    model.OtherTeams.Add(Tuple.Create(team, under17s, open));
            }
            //model.OtherTeams = otherTeams.Where(t => t.Item2 || t.Item3).ToList();

            if(!model.Event.EventTeamEntries.Any())
            {
                var similarEvents = db.Events.Where(e => e.EventTypeId == model.Event.EventTypeId && e.Id != model.Event.Id && e.EventTeamEntries.Count > 0);
                ViewBag.PreviousEventId = new SelectList(similarEvents, "Id", "EventName");
            }
            return View(model);
        }


        // GET: EventTeamEntry/Add
        public ActionResult AddTeam(int eventId, int? teamId, bool under17s = false)
        {
            var model = new ParticipatingTeam();
            model.Event = db.Events.Find(eventId);
            model.Under17s = under17s;
            model.EventType = model.Event.EventType1.Type;

            if(teamId != null)
                model.Team = db.Teams.Find(teamId);
            //var participants = model.Event.EventTeamEntries.SelectMany(e => e.EventRiderEntries).ToList();

            //var availableRiders = db.Riders.Where(r => !participants.Any(p => r.Id == p.RiderId)).Select(x=>new {Id = x.Id, Label=x.FullName});
            //var availableRiders = db.Riders.Except(participants.Select(p=>p.Rider)).Select(x=>new {Id = x.Id, Label=x.FullName});
            //var availablePonies = db.Ponies.Where(x => !participants.Any(p => x.Id == p.PonyId)).Select(x => new { Id = x.Id, Label = x.Name });

            //var teamEntries = db.EventTeamEntries.Select(x => new { Id = x.Id, Label = x.Event.EventName + "/" + x.Team.TeamName });
            var availableRiders = db.Riders.Select(x => new { Id = x.Id, Label = x.FullName }).OrderBy(r=>r.Label);
            var availablePonies = db.Ponies.Select(x => new { Id = x.Id, Label = x.Name }).OrderBy(p=>p.Label);
            //var noRider = new[] { new { Id = -1, Label = "--No Rider--" } };
            model.AvailableRiders = new SelectList(availableRiders, "Id", "Label");
            model.AvailablePonies = new SelectList(availablePonies, "Id", "Label");

            //ViewBag.TeamId = new SelectList(db.Teams, "Id", "TeamName");
            //ViewBag.EventId = new SelectList(db.Events, "Id", "EventName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTeam([Bind(Include = "Id,TeamId,EventId,Under17s,Rider1Id,Pony1Id,Rider2Id,Pony2Id,Rider3Id,Pony3Id,Rider4Id,Pony4Id,Rider5Id,Pony5Id")] ParticipatingTeam model)
        {
            if (ModelState.IsValid)
            {
                var teamEntry = new EventTeamEntry() { EventId = model.EventId, TeamId = model.TeamId, Under17s = model.Under17s };
                db.EventTeamEntries.Add(teamEntry);
                db.SaveChanges();
                var r1 = new EventRiderEntry() { EventTeamEntry = teamEntry, RiderId = model.Rider1Id.Value, PonyId = model.Pony1Id.Value };
                db.EventRiderEntries.Add(r1);
                if (model.Rider2Id > 0)
                {
                    var r2 = new EventRiderEntry() { EventTeamEntry = teamEntry, RiderId = model.Rider2Id.Value, PonyId = model.Pony2Id.Value };
                    db.EventRiderEntries.Add(r2);
                }
                if (model.Rider3Id > 0)
                {
                    var r3 = new EventRiderEntry() { EventTeamEntry = teamEntry, RiderId = model.Rider3Id.Value, PonyId = model.Pony3Id.Value };
                    db.EventRiderEntries.Add(r3);
                }
                if (model.Rider4Id > 0)
                {
                    var r4 = new EventRiderEntry() { EventTeamEntry = teamEntry, RiderId = model.Rider4Id.Value, PonyId = model.Pony4Id.Value };
                    db.EventRiderEntries.Add(r4);
                }
                //db.EventRiderEntries.AddRange(new EventRiderEntry[] { r1, r2, r3, r4 });
                if (model.Rider5Id > 0)
                {
                    var r5 = new EventRiderEntry() { EventTeamEntry = teamEntry, RiderId = model.Rider5Id.Value, PonyId = model.Pony5Id.Value };
                    db.EventRiderEntries.Add(r5);
                }
                db.SaveChanges();
                return RedirectToAction("Participants", new { id = model.EventId });
            }

            //ViewBag.TeamId = new SelectList(db.Teams, "Id", "TeamName", model.TeamId);
            //ViewBag.EventId = new SelectList(db.Events, "Id", "EventName", model.EventId);
            return View(model);
        }

        public ActionResult Results(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = new EventResults();
            model.Teams= new List<TeamEntryResult>();
            var e = db.Events.Find(id);
            model.EventType = e.EventType1.Type;
            
            foreach(var entry in e.EventTeamEntries.OrderBy(te=>te.Team==null?"":te.Team.TeamName))
            {
                var line = new TeamEntryResult();
                line.EventTeamEntryId = entry.Id;
                line.TeamName = entry.Team==null?"Independent Entrant": entry.Team.TeamName;
                line.EntryCategory = entry.Under17s ? "Under 17s" : "Open";
                line.Points = entry.Points.GetValueOrDefault();
                
                var riders = entry.EventRiderEntries.OrderBy(er => er.Id);

                    line.Rider1Participated = riders.ElementAtOrDefault(0).Participated.GetValueOrDefault(true);
                    line.Rider1Name = riders.ElementAtOrDefault(0).Rider.FullName;

                if (riders.Count() > 1)
                {
                line.Rider2Participated = riders.ElementAtOrDefault(1).Participated.GetValueOrDefault(true);
                line.Rider2Name = riders.ElementAtOrDefault(1).Rider.FullName;
                }
                if (riders.Count() > 2)
                {
                    line.Rider3Participated = riders.ElementAtOrDefault(2).Participated.GetValueOrDefault(true);
                    line.Rider3Name = riders.ElementAtOrDefault(2).Rider.FullName;
                }
                if (riders.Count() > 3)
                {
                    line.Rider4Participated = riders.ElementAtOrDefault(3).Participated.GetValueOrDefault(true);
                    line.Rider4Name = riders.ElementAtOrDefault(3).Rider.FullName;
                }
                if (riders.Count() == 5)
                {
                    line.Rider5Participated = riders.ElementAtOrDefault(4).Participated.GetValueOrDefault(true);
                    line.Rider5Name = riders.ElementAtOrDefault(4).Rider.FullName;
                }
                model.Teams.Add(line);
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Results(EventResults model)
        {
            if (ModelState.IsValid)
            {
                foreach (var line in model.Teams)
                {
                    var entry = db.EventTeamEntries.Find(line.EventTeamEntryId);
                    entry.Points = line.Points;
                    var riders = entry.EventRiderEntries.OrderBy(er => er.Id);
                    riders.ElementAt(0).Participated = line.Rider1Participated;
                    if (riders.Count() > 1)
                        riders.ElementAt(1).Participated = line.Rider2Participated;
                    if (riders.Count() > 2)
                        riders.ElementAt(2).Participated = line.Rider3Participated;
                    if (riders.Count() > 3)
                        riders.ElementAt(3).Participated = line.Rider4Participated;
                    if (riders.Count() == 5)
                        riders.ElementAt(4).Participated = line.Rider5Participated;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
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
            ViewBag.EventTypeID = new SelectList(db.EventTypes, "Id", "Type");
            return View();
        }

        // POST: Event/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EventName,EventDate,EventTypeId,EventType,EventPriority,SeasonID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SeasonID = new SelectList(db.Seasons, "Id", "SeasonName", @event.SeasonID);
            ViewBag.EventTypeID = new SelectList(db.EventTypes, "Id", "Type", @event.EventTypeId);
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
            var type = @event.EventTypeId;
            ViewBag.SeasonID = new SelectList(db.Seasons, "Id", "SeasonName", @event.SeasonID);
            ViewBag.EventTypeID = new SelectList(db.EventTypes, "Id", "Type", type);
            //if(!@event.EventTeamEntries.Any())
            //{
            //    var similarEvents = db.Events.Where(e => e.EventTypeId == type && e.Id != @event.Id && e.EventTeamEntries.Count > 0);
            //    ViewBag.PreviousEvents = new SelectList(similarEvents, "Id", "EventName");
            //}
            return View(@event);
        }

        [HttpPost, ActionName("CopyPrevious")]
        [ValidateAntiForgeryToken]
        public ActionResult CopyPrevious(int Id, int PreviousEventId)
        {
            Event thisEvent = db.Events.Find(Id);
            Event previousEvent = db.Events.Include("EventTeamEntries.EventRiderEntries").First(e => e.Id == PreviousEventId);
            var POS = 1;
            foreach(var previousEntry in previousEvent.EventTeamEntries)
            {
                System.Diagnostics.Debug.Print("Entry " + (POS++) + " of " + previousEvent.EventTeamEntries.Count); 
                var newEntry = new EventTeamEntry();
                thisEvent.EventTeamEntries.Add(newEntry);
                //newEntry.Event = thisEvent;
                newEntry.TeamId = previousEntry.TeamId;
                newEntry.Under17s = previousEntry.Under17s;
                newEntry.EventRiderEntries = new List<EventRiderEntry>();
                foreach(var previousRider in previousEntry.EventRiderEntries)
                {
                    var newRider = new EventRiderEntry();
                    newEntry.EventRiderEntries.Add(newRider);
                    newRider.PonyId = previousRider.PonyId;
                    newRider.RiderId = previousRider.RiderId;
                }
            }
            //db.Events.Remove(@event);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Event/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EventName,EventDate,EventTypeId,EventType,EventPriority,SeasonID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SeasonID = new SelectList(db.Seasons, "Id", "SeasonName", @event.SeasonID);
            ViewBag.EventTypeID = new SelectList(db.EventTypes, "Id", "Type", @event.EventTypeId);
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
