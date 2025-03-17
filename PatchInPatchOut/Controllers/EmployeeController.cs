using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatchInPatchOut.Data;
using PatchInPatchOut.ViewModel;

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
                .Select(x => new AttendanceModel()
                {
                    AttendanceId = x.AttendanceId,
                    UserId = x.UserId,
                    UserDetails = new UserModel()
                    {
                        UserId = x.UserDetails.UserId,
                        NameOfUser = x.UserDetails.NameOfUser,
                        Department = x.UserDetails.Department,
                        UserName = x.UserDetails.UserName,
                        ShiftStart = x.UserDetails.ShiftStart,
                        ShiftEnd = x.UserDetails.ShiftEnd,
                        Role = x.UserDetails.Role
                    },
                    PatchIn = x.PatchIn,
                    PatchOut = x.PatchOut,
                    IsPresent = x.IsPresent,
                    QRCode = x.QRCode,
                    QRGeneratedDate = x.QRGeneratedDate,
                })
                .OrderByDescending(a => a.PatchIn)
                .ToList();

            return View(myAttendanceRecords);
        }


        public IActionResult ScanQRCode()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessQRCode(string scannedQRCode)
        {
            var today = DateTime.Now.Date;
            var userId = HttpContext.Session.GetInt32("UserId"); // Get the logged-in user

            if (userId == null)
            {
                return Json(new { success = false, message = "User not found. Please log in again." });
            }


            var todayQR = await _context.Attendances.FirstOrDefaultAsync(a => a.QRGeneratedDate.Date == today);

            if (todayQR == null || todayQR.QRCode != scannedQRCode)
            {
                return Json(new { success = false, message = "Invalid QR Code." });
            }

            var attendance = await _context.Attendances
                .Where(a => a.UserId == userId && a.QRGeneratedDate.Date == today)
                .OrderByDescending(a => a.PatchIn)
                .FirstOrDefaultAsync();

            if (attendance == null)
            {
                var newAttendance = new Attendance
                {
                    UserId = userId.Value,
                    PatchIn = DateTime.Now,
                    QRCode = scannedQRCode,
                    QRGeneratedDate = DateTime.Now,
                    IsPresent = true
                };

                await _context.Attendances.AddAsync(newAttendance);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Check-in successful!" });

            }

            else
            {
                attendance.PatchOut = DateTime.Now;
                _context.Attendances.Update(attendance);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Check-out successful!" });


            }





        }

    }
}
