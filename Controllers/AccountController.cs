using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TennisApp.Models;
using System.Threading.Tasks;
using TennisApp.Data;

namespace TennisApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        // Inject ApplicationDbContext into the constructor
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;  // Assign the context
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign role based on the status
                    if (model.Status == "Member")
                    {
                        await _userManager.AddToRoleAsync(user, "Member");
                    }
                    else if (model.Status == "Coach")
                    {
                        await _userManager.AddToRoleAsync(user, "Coach");

                        // Automatically create a Coach profile with generated CoachID
                        var coach = new Coach
                        {
                            Email = user.Email,
                            FirstName = "",
                            LastName = "",
                            Biography = ""
                        };

                        _context.Coaches.Add(coach);  // Add coach to the database
                        await _context.SaveChangesAsync();  // Save changes
                    }
                    else if (model.Status == "Admin")
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");

                        var member = new Member
                        {
                            MemberID = user.Id,  // UserID is MemberID
                            FirstName = "",
                            LastName = "",
                            Email = user.Email
                        };

                        _context.Members.Add(member);
                        await _context.SaveChangesAsync();
                    }

                    // Sign the user in
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect to role-specific dashboard
                    if (model.Status == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (model.Status == "Coach")
                    {
                        return RedirectToAction("Index", "Coaches");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Members");
                    }
                }

                // If there are errors, add them to the ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        // Logout Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();  // Sign the user out
            return RedirectToAction("Index", "Home");  // Redirect to home page after logout
        }
    }
}
