using Microsoft.AspNetCore.Mvc;
using TennisApp.Data;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TennisApp.Models;

namespace TennisApp.Controllers
{
    [Authorize(Roles = "Member")]
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MembersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // View all available schedules
        public IActionResult Index()
        {
            var schedules = _context.Schedules.Include(s => s.Coach).ToList();
            return View(schedules);
        }

        // Handle member enrollment into a schedule
        [HttpPost]
        public async Task<IActionResult> Enroll(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var member = _context.Members.FirstOrDefault(m => m.Email == user.Email);

            if (member != null)
            {
                var schedule = _context.Schedules.FirstOrDefault(s => s.ScheduleID == id);
                if (schedule != null)
                {
                    var enrollment = new Enrollment
                    {
                        MemberID = member.MemberID,
                        ScheduleID = schedule.ScheduleID
                    };
                    _context.Enrollments.Add(enrollment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(MyEnrollments));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // View member's enrollments
        public IActionResult MyEnrollments()
        {
            var user = _userManager.GetUserAsync(User).Result;
            var member = _context.Members.FirstOrDefault(m => m.Email == user.Email);
            var enrollments = _context.Enrollments
                                      .Include(e => e.Schedule)
                                      .ThenInclude(s => s.Coach)
                                      .Where(e => e.MemberID == member.MemberID)
                                      .ToList();
            return View(enrollments);
        }
    }
}
