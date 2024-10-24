namespace TennisApp.Models
{
    public class Schedule
    {
        public int ScheduleID { get; set; }
        public string EventName { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }

        public string CoachID { get; set; }
        public Coach Coach { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
