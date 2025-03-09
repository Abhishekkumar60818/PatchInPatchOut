using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatchInPatchOut.Data;
using PatchInPatchOut.Models;
using QRCoder;
using static PatchInPatchOut.Services.encriptPassword;


namespace PatchInPatchOut.Controllers
{

    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult EmployeeReportDashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var attendanceRecords = _context.Attendances
                .Include(a => a.UserDetails)
                .Where(a => a.UserDetails != null && a.UserDetails.Role != UserRole.Admin)
                .Select(a => new
                {
                    UserId = a.UserDetails.UserId,
                    UserName = a.UserDetails.UserName,
                    NameOfUser = a.UserDetails.NameOfUser,
                    PatchIn = (DateTime?)a.PatchIn,
                    PatchOut = (DateTime?)a.PatchOut,
                    IsPresent = a.IsPresent,
                    a.QRGeneratedDate
                })
                .ToList();

            return View(attendanceRecords);
        }

        public IActionResult AddEmployee()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddEmployee(User user)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var exitingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
            if (exitingUser != null)
            {
                ViewBag.Error = "Username already exists. Please use a different Username.";
                return View(user);  // This allows user to see their input with error
            }

            string hashedPassword = HashPassword(user.Password);
            _context.Users.Add(new User
            {
                NameOfUser = user.NameOfUser,
                UserName = user.UserName,
                Department = user.Department,
                Password = hashedPassword,
                Role = UserRole.Employee
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = "Employee added successfully!";
            return RedirectToAction("AddEmployee");
        }

        public IActionResult ShowAllEmployee()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var employees = _context.Users
                .Where(u => u.Role == UserRole.Employee) // Exclude Admins
                .ToList();

            return View(employees); // Pass only employees to the view
        }


        public IActionResult GenerateQrCode()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var now = DateTime.Now;
            var today = now.Date;

            var existingQr = _context.Attendances.FirstOrDefault(a => a.QRGeneratedDate.Date == today);

            string qrCodeData;
            DateTime qrGeneratedDateTime;

            if (existingQr == null)
            {
                qrCodeData = Guid.NewGuid().ToString();

                var adminUser = _context.Users.FirstOrDefault(u => u.Role == UserRole.Admin);
                if (adminUser == null)
                {
                    return BadRequest("No admin user found. Cannot generate QR Code.");
                }

                var newQr = new Attendance
                {
                    QRCode = qrCodeData,
                    QRGeneratedDate = now,
                    UserId = adminUser.UserId
                };

                _context.Attendances.Add(newQr);
                _context.SaveChanges();
                qrGeneratedDateTime = now;
            }
            else
            {
                qrCodeData = existingQr.QRCode;
                qrGeneratedDateTime = existingQr.QRGeneratedDate;
            }

            string qrCodeBase64 = GenerateQRCode(qrCodeData);
            ViewBag.QRCodeImage = qrCodeBase64;
            ViewBag.GeneratedDateTime = qrGeneratedDateTime.ToString("yyyy-MM-dd HH:mm:ss");

            return View();
        }

        private string GenerateQRCode(string text)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeImage = qrCode.GetGraphic(20);
                return Convert.ToBase64String(qrCodeImage);
            }
        }

    }
}
