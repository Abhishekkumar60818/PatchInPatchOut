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
        public UserRole Role { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
