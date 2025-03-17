using System.ComponentModel.DataAnnotations.Schema;

namespace PatchInPatchOut.Models
{
    public enum UserRole
    {
        Admin,
        Employee
    }

    public class User
    {
        public int UserId { get; set; }
        public string NameOfUser { get; set; }


        public string Department { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }  // Store hashed passwords
        public TimeOnly? ShiftStart { get; set; }
        public TimeOnly? ShiftEnd { get; set; }

        public int? MinimumWorking()
        {
            if (ShiftEnd.HasValue && ShiftStart.HasValue)
            {
                TimeSpan duration = ShiftEnd.Value - ShiftStart.Value;
                int spacingIn = SpacingShiftIn ?? 0;
                int spacingOut = SpacingShiftOut ?? 0;

                return (int)duration.TotalMinutes - (spacingIn + spacingOut);
            }
            return null;
        }

        [NotMapped] public int? ShiftInHours { get; set; }
        [NotMapped] public int? ShiftInMinutes { get; set; }
        [NotMapped] public int? ShiftOutHours { get; set; }
        [NotMapped] public int? ShiftOutMinutes { get; set; }
        public int? SpacingShiftIn { get; set; }
        public int? SpacingShiftOut { get; set; }
        public UserRole Role { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
