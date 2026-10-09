using Microsoft.EntityFrameworkCore;
using HospitalSystem.Models;

namespace HospitalSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Prevent Double Booking
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.DoctorId, a.Date, a.Time })
                .IsUnique();
        }
    }
}
