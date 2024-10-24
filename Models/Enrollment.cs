namespace TennisApp.Models
{
    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        public string MemberID { get; set; }
        public Member Member { get; set; }

        public int ScheduleID { get; set; }
        public Schedule Schedule { get; set; }
    }
}
