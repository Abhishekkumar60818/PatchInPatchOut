using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatchInPatchOut.Data;
using PatchInPatchOut.Models;
using static PatchInPatchOut.Services.encriptPassword;




namespace PatchInPatchOut.Controllers
{

    public class AccountController : Controller
    {

        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string UserName, string password)
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Username and password are required.";
                return View();
            }

            string hashedPassword = HashPassword(password.Trim());
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == UserName.Trim().ToLower() && u.Password == hashedPassword);

            if (user == null)
            {
                ViewBag.Error = "Invalid credentials";
                return View();
            }

            // Store user data in session
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());





            if (user.Role == UserRole.Admin)
                return RedirectToAction("EmployeeReportDashboard", "Admin");
            else
                return RedirectToAction("MyReport", "Employee");


        }


        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login"); // Redirect to login if no user is logged in
            }

            HttpContext.Session.Clear();
            TempData["LogoutMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }


    }
}
