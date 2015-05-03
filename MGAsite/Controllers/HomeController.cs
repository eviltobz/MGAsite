using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MGAsite.Models;

namespace MGAsite.Controllers
{
    public class HomeController : Controller
    {
        private MgaEntities db = new MgaEntities();

        public ActionResult Index(int? id)
        {
            var model = new OrderOfMerit();
            if (id.HasValue)
                model.SelectedSeason = db.Seasons.Find(id);
            else
                model.SelectedSeason = db.Seasons.First();

            model.Seasons = new SelectList(db.Seasons, "Id", "SeasonName", model.SelectedSeason.Id);

            var events = model.SelectedSeason.Events.OrderBy(e => e.EventDate);
            model.Events = events.Select(e => e.EventName).ToArray();
            model.EventCount = model.Events.Length;

            var riders = new Dictionary<int, OrderOfMerit.RiderLine>();
            for (int i = 0; i < model.EventCount; i++)
            {
                var race = events.ElementAt(i);
                var eventRiders = race.EventTeamEntries.SelectMany(e => e.EventRiderEntries.Select(r => new { Points = e.Points, Rider = r.Rider, Participated = r.Participated }));

                foreach (var rider in eventRiders)
                {
                    if (!riders.ContainsKey(rider.Rider.Id))//  riderLine == null)
                    {
                        riders[rider.Rider.Id] = new OrderOfMerit.RiderLine();
                        riders[rider.Rider.Id].Name = rider.Rider.FullName;
                        riders[rider.Rider.Id].EventResults = events.Select(e => new Tuple<int?, bool>(null, true)).ToArray();
                    }
                    var riderLine = riders[rider.Rider.Id];
                    riderLine.EventResults[i] = new Tuple<int?, bool>(rider.Points, rider.Participated??true);
                    if (rider.Points.HasValue)
                    {
                        riderLine.TotalPoints += rider.Points.Value;
                        riderLine.TotalEvents++;
                    }
                }

            }
            model.Riders = riders.Values.OrderBy(r=>r.Name);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeSeason(int id)
        {
            return Index(id);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult ManageData()
        {
            return View();
        }
    }
}