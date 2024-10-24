using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TennisApp.Data;
using TennisApp.Models;

namespace TennisApp.Controllers
{
    [Authorize(Roles = "Coach")]
    public class CoachesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CoachesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var coach = _context.Coaches.FirstOrDefault(c => c.Email == user.Email);

            if (coach == null)
            {
                // Redirect to a page where the coach can create a profile instead of showing an error
                return RedirectToAction("CreateProfile");
            }

            var schedules = _context.Schedules
                .Where(s => s.CoachID == coach.CoachID && s.Date >= DateTime.Now)
                .OrderBy(s => s.Date)
                .ToList();

            var viewModel = new CoachDashboardViewModel
            {
                Coach = coach,
                Schedules = schedules
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult CreateProfile()
        {
            return View();  // Display a form to create a new coach profile
        }

        [HttpPost]
        public async Task<IActionResult> CreateProfile(Coach model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var coach = new Coach
                {
                    Email = user.Email,
                    CoachID = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Biography = model.Biography
                };

                _context.Coaches.Add(coach);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }


        // View or Edit Profile
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            var coach = _context.Coaches.FirstOrDefault(c => c.Email == user.Email);

            if (coach == null)
            {
                return RedirectToAction("Error");  // Handle the error if the coach profile doesn't exist
            }

            return View(coach);  // Display the coach's profile
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(Coach model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var coach = _context.Coaches.FirstOrDefault(c => c.Email == user.Email);

                if (coach != null)
                {
                    // Update existing profile
                    coach.FirstName = model.FirstName;
                    coach.LastName = model.LastName;
                    coach.Biography = model.Biography;

                    _context.Coaches.Update(coach);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        public IActionResult Error()
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                
            };

            return View(errorViewModel);
        }

        public class ErrorViewModel
        {
            public string RequestId { get; set; }
            public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        }
    }
}
