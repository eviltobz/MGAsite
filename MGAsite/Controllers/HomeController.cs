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
                    var riderLine = riders[rider.Rider.Id];
                    if (riderLine == null)
                    {
                        riderLine = new OrderOfMerit.RiderLine();
                        riderLine.Name = rider.Rider.FullName;
                        riderLine.EventResults = new Tuple<int?, bool?>[model.EventCount];
                    }
                    riderLine.EventResults[i] = new Tuple<int?, bool?>(rider.Points, rider.Participated);
                }

            }

            return View(model);
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