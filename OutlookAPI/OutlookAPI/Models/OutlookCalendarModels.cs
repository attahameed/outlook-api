using System.Collections.Generic;

namespace OutlookAPI.Models
{
    public class ReceivedCalendarsData
    {
        public List<OutlookCalendar> value { get; set; }
    }

    public class OutlookCalendar
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Permissions { get; set; }

        public bool isDefaultCalendar { get; set; }
        public bool canEdit { get; set; }
        public Owner owner { get; set; }
    }

    public class Owner
    {
        public string name { get; set; }
        public string address { get; set; }
    }
}