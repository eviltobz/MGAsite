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

        [HttpGet]
        public ActionResult Index()
        {
            return ShowIndex(null, false, false);
        }
        private string GetMean(IEnumerable<int> points)
        {
            decimal sum = points.Sum();
            decimal count = points.Count();
            if (count == 0)
                return "N/A";
            return decimal.Round(sum / count, 1).ToString();
        }

        private ActionResult ShowIndex(int? id, bool under17s, bool showAll)
        {
            var model = new OrderOfMerit();
            model.Under17s = under17s;
            model.ShowAll = showAll;
            Season selectedSeason;
            if (id.HasValue)
                selectedSeason = db.Seasons.Find(id);
            else
                selectedSeason = db.Seasons.First();
            model.SelectedSeasonId = selectedSeason.Id;

            model.Seasons = new SelectList(db.Seasons, "Id", "SeasonName", selectedSeason.Id);

            var events = db.Events.Include("EventTeamEntries.EventRiderEntries.Rider").Where(e=>e.SeasonID == selectedSeason.Id).OrderBy(e => e.EventDate).ToArray();
            model.Events = events.OrderBy(e=>e.EventDate).Select(e => e.EventName).ToArray();
            model.EventCount = model.Events.Length;

            //var seasonEvents = db.Events.Where(e => e.SeasonID == selectedSeason.Id);
            var completedEvents = events.Where(e => e.EventTeamEntries.Any(te => te.Points != null));
            if (showAll)
                minimumEvents = 0;
            else
                minimumEvents = completedEvents.Count() - 1;
            var eventCount = events.Count();
            var completedCount = completedEvents.Count();
            var seasonCompleted = (eventCount == completedCount);

            var riders = new Dictionary<int, OrderOfMerit.RiderLine>();
            for (int i = 0; i < model.EventCount; i++)
            {
                var race = events.ElementAt(i);
                var eventRiders = race.EventTeamEntries.Where(e=>e.Under17s == under17s).SelectMany(e => 
                    e.EventRiderEntries.Select(r => new { Points = e.Points, Rider = r.Rider, Participated = r.Participated }));

                foreach (var rider in eventRiders)
                {
                    if (!riders.ContainsKey(rider.Rider.Id))//  riderLine == null)
                    {
                        riders[rider.Rider.Id] = new OrderOfMerit.RiderLine();
                        riders[rider.Rider.Id].Name = rider.Rider.FullName;
                        riders[rider.Rider.Id].Nationality = rider.Rider.Nationality.Name;
                        riders[rider.Rider.Id].EventResults = events.Select(e => new Tuple<int?, bool>(null, true)).ToArray();
                    }
                    var riderLine = riders[rider.Rider.Id];
                    riderLine.EventResults[i] = new Tuple<int?, bool>(rider.Points, rider.Participated ?? true);
                }

            }
            foreach (var entry in riders.Values)
            {
                var scores = new List<int>();
                var competedInAll = true;
                foreach (var race in entry.EventResults)
                {
                    if (race.Item1.HasValue)
                        scores.Add(race.Item1.Value);
                    else
                        competedInAll = false;
                }
                entry.MeanPoints = GetMean(scores);
                if (completedCount > 1 && competedInAll)
                {
                        var subscores = scores.OrderByDescending(s => s).Take(completedCount - 1);
                        entry.ExclusiveMeanPoints = GetMean(subscores);
                }
                else
                    entry.ExclusiveMeanPoints = entry.MeanPoints;
            }

            /*
            int a =events.Where(e=>e.EventTeamEntries.Any(ete=>ete.Points != null)).Count();
            if (a > 3 && !showAll)
                minimumEvents = a / 2;
            else
                minimumEvents = 0;
            */


            model.Riders = riders.Values.Where(RiderQualifies).OrderBy(r => r.Name);

            return View(model);
        }
        private int minimumEvents = 0;
        private bool RiderQualifies(OrderOfMerit.RiderLine arg)
        {
            return arg.EventResults.Count(e => e.Item1 != null) >= minimumEvents;
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MGAsite.Models.OrderOfMerit model)
        {
            return ShowIndex(model.SelectedSeasonId, model.Under17s, model.ShowAll);
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