//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MGAsite.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EventTeamEntry
    {
        public EventTeamEntry()
        {
            this.EventRiderEntries = new HashSet<EventRiderEntry>();
        }
    
        public int Id { get; set; }
        public Nullable<int> TeamId { get; set; }
        public int EventId { get; set; }
        public Nullable<int> Points { get; set; }
        public bool Under17s { get; set; }
    
        public virtual ICollection<EventRiderEntry> EventRiderEntries { get; set; }
        public virtual Team Team { get; set; }
        public virtual Event Event { get; set; }
    }
}
