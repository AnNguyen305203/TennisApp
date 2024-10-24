namespace TennisApp.Models
{
    public class Coach
    {
        public string CoachID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Biography { get; set; }
        public string Email { get; set; }

        public ICollection<Schedule> Schedules { get; set; }
    }
}
