using PatchInPatchOut.Models;

namespace PatchInPatchOut.ViewModel
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string NameOfUser { get; set; }
        public string Department { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public TimeOnly? ShiftStart { get; set; }
        public TimeOnly? ShiftEnd { get; set; }
        public UserRole Role { get; set; }
    }
}
