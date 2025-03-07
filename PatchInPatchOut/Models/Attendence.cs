using PatchInPatchOut.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Attendance
{
    [Key]
    public int AttendanceId { get; set; }


    public int? UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User? UserDetails { get; set; }

    public DateTime? PatchIn { get; set; }
    public DateTime? PatchOut { get; set; }
    public bool IsPresent { get; set; } = false;

    public string? QRCode { get; set; }
    public DateTime QRGeneratedDate { get; set; }

}
