using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OutlookAPI.Models
{
    public class ReceivedEventsData
    {
        public List<OutlookEvent> value { get; set; }
    }

    public class OutlookEvent
    {
        public string id { get; set; }

        [Required]
        public string subject { get; set; }

        public string bodyPreview { get; set; }


        public Body body { get; set; }

        public Start start { get; set; }


        public End end { get; set; }

        public bool isAllDay { get; set; }
        
        public bool isCancelled { get; set; }
        
        public bool isOrganizer { get; set; }
    }

    public class Body
    {
        public string contentType { get; set; }
        
        public string content { get; set; }
    }

    public class Start
    {
        [Required]
        public DateTime dateTime { get; set; }

        public string timeZone { get; set; }
    }

    public class End
    {
        [Required]
        public DateTime dateTime { get; set; }

        public string timeZone { get; set; }
    }


    public class Organizer
    {
        public Emailaddress emailAddress { get; set; }
    }

    public class Emailaddress
    {
        public string name { get; set; }
        public string address { get; set; }
    }
}