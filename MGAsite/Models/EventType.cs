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
    
    public partial class EventType
    {
        public EventType()
        {
            this.Events = new HashSet<Event>();
        }
    
        public int Id { get; set; }
        public string Type { get; set; }
    
        public virtual ICollection<Event> Events { get; set; }
    }
}
