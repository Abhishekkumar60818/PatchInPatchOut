namespace PatchInPatchOut.ViewModel
{
    public class AttendanceModel
    {
        public int AttendanceId { get; set; }
        public int? UserId { get; set; }
        public UserModel? UserDetails { get; set; }
        public DateTime? PatchIn { get; set; }
        public DateTime? PatchOut { get; set; }
        public bool IsPresent { get; set; } = false;
        public string? QRCode { get; set; }
        public DateTime QRGeneratedDate { get; set; }
    }
}
