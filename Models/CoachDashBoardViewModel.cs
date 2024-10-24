using System.Collections.Generic;

namespace TennisApp.Models
{
    public class CoachDashboardViewModel
    {
        public Coach Coach { get; set; }  // Coach's profile information
        public IEnumerable<Schedule> Schedules { get; set; }  // Coach's upcoming schedules
    }
}
