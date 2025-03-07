using Microsoft.EntityFrameworkCore;
using PatchInPatchOut.Models;
using static PatchInPatchOut.Services.encriptPassword;

namespace PatchInPatchOut.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }  // Ensure correct model name
        public DbSet<Attendance> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.UserDetails)
                .WithMany(u => u.Attendances)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed Admin User
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    NameOfUser = "Admin",
                    UserName = "admin@example.com",
                    Department = "Admin",
                    Password = HashPassword("Admin@123"),
                    Role = UserRole.Admin
                }
            );
        }
    }
}
