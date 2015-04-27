using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

using System.Web.Mvc;

namespace MGAsite.Models
{
    public class EventParticipantsViewModel
    {
        public Event Event { get; set; }
        public IList<EventTeamEntry> ParticipatingTeams { get; set; }
        public IList<Team> OtherTeams { get; set; }
    }

    public class ParticipatingTeam
    {
        public Event Event { get; set; }
        public Team Team { get; set; }
        public int EventId { get; set; }
        public int TeamId { get; set; }


        public SelectList AvailableRiders { get; set; }
        public SelectList AvailablePonies { get; set; }

        public int? Rider1Id { get; set; }
        public int? Pony1Id { get; set; }

        public int? Rider2Id { get; set; }
        public int? Pony2Id { get; set; }

        public int? Rider3Id { get; set; }
        public int? Pony3Id { get; set; }

        public int? Rider4Id { get; set; }
        public int? Pony4Id { get; set; }

        public int? Rider5Id { get; set; }
        public int? Pony5Id { get; set; }
    }
    
    public class Results
    {
        public Event Event { get; set; }

        public int EventTeamEntryId { get; set; }
        public int Points { get; set; }
        public bool Rider1Participated { get; set; }
        public bool Rider2Participated { get; set; }
        public bool Rider3Participated { get; set; }
        public bool Rider4Participated { get; set; }
        public bool Rider5Participated { get; set; }
    }

}