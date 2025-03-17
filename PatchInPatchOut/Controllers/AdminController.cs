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
        public IActionResult EmployeeReportDashboard(User user)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var attendanceRecords = _context.Attendances
                .Include(a => a.UserDetails)
                .Where(a => a.UserDetails != null && a.UserDetails.Role != UserRole.Admin)
                .OrderByDescending(a => a.PatchIn)
                .ToList();
            return View(attendanceRecords);
        }

        public IActionResult AddEmployee()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddEmployee(User model)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);
            if (existingUser != null)
            {
                ViewBag.Error = "Username already exists. Please use a different Username.";
                return View(model);  // Return input with error message
            }

            // Ensure password is not null before hashing
            string hashedPassword = !string.IsNullOrEmpty(model.Password) ? HashPassword(model.Password) : throw new ArgumentException("Password cannot be empty.");

            var user = new User
            {
                NameOfUser = model.NameOfUser,
                UserName = model.UserName,
                Department = model.Department,
                Password = hashedPassword,
                Role = UserRole.Employee,
                ShiftStart = model.ShiftStart,
                ShiftEnd = model.ShiftEnd,
                SpacingShiftIn = (model.ShiftInHours ?? 0) * 60 + (model.ShiftInMinutes ?? 0),
                SpacingShiftOut = (model.ShiftOutHours ?? 0) * 60 + (model.ShiftOutMinutes ?? 0)
            };

            // Save to database
            await _context.Users.AddAsync(user);
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
                .OrderByDescending(u => u.UserId)
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
