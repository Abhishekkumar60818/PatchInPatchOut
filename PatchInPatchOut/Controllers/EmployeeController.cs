using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatchInPatchOut.Data;

namespace PatchInPatchOut.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult MyReport()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var myAttendanceRecords = _context.Attendances
                .Include(a => a.UserDetails)
                .Where(a => a.UserId == userId)
                .Select(a => new
                {
                    UserName = a.UserDetails.UserName,
                    NameOfUser = a.UserDetails.NameOfUser,
                    Department = a.UserDetails.Department,
                    PatchIn = (DateTime?)a.PatchIn,
                    PatchOut = (DateTime?)a.PatchOut,
                    IsPresent = a.IsPresent
                })
                .ToList();

            return View(myAttendanceRecords);
        }


        public IActionResult ScanQRCode()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProcessQRCode(string scannedQRCode)
        {
            var today = DateTime.Now.Date;
            var userId = HttpContext.Session.GetInt32("UserId"); // Get the logged-in user

            if (userId == null)
            {
                return Json(new { success = false, message = "User not found. Please log in again." });
            }


            var todayQR = _context.Attendances.FirstOrDefault(a => a.QRGeneratedDate.Date == today);

            if (todayQR == null && todayQR.QRCode != scannedQRCode)
            {
                return Json(new { success = false, message = "Invalid QR Code." });
            }

            var attendance = _context.Attendances
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.PatchIn)
                .FirstOrDefault();

            var now = DateTime.Now;

            if (attendance == null || attendance.PatchIn == null)
            {
                var newAttendance = new Attendance
                {
                    UserId = userId.Value,
                    PatchIn = now,
                    QRCode = todayQR.QRCode,
                    QRGeneratedDate = now,
                    IsPresent = true
                };

                _context.Attendances.Add(newAttendance);
            }
            else
            {
                attendance.PatchOut = now;
                _context.Attendances.Update(attendance);
            }

            _context.SaveChanges();
            return Json(new { success = true, message = "Attendance updated successfully!" });
        }

    }
}
