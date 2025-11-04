using Domain.Entities;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AvailabilityBlock> AvailabilityBlocks { get; set; }
        public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // APPOINTMENTS
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("Appointment");
                entity.HasKey(e => e.AppointmentId);
                entity.Property(c => c.AppointmentId).ValueGeneratedOnAdd(); //autogenerada
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.PatientId).IsRequired();
                entity.Property(e => e.StartTime).IsRequired();
                entity.Property(e => e.EndTime).IsRequired();
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.MeetingURL).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
                // Self Reference: cita original si está reprogramada
                entity.HasOne(e => e.OriginalAppointment).WithMany().HasForeignKey(e => e.OriginalAppointmentId).OnDelete(DeleteBehavior.Restrict);
            });

            // AVAILABILITY BLOCKS
            modelBuilder.Entity<AvailabilityBlock>(entity =>
            {
                entity.ToTable("AvailabilityBlock");
                entity.HasKey(e => e.Id);
                entity.Property(c => c.Id).ValueGeneratedOnAdd(); //autogenerada
                entity.Property(e => e.DoctorId).IsRequired();
                entity.Property(e => e.StartTime).IsRequired();
                entity.Property(e => e.EndTime).IsRequired();
                entity.Property(e => e.IsBlock).IsRequired();
                entity.Property(e => e.Reason).HasMaxLength(255);
                entity.Property(e => e.Note).HasMaxLength(1000);
                entity.Property(e => e.AllDay).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
                entity.Property(e => e.Deleted).IsRequired().HasDefaultValue(false);
            });

            // DOCTOR AVAILABILITIES
            modelBuilder.Entity<DoctorAvailability>(entity =>
            {
                entity.ToTable("DoctorAvailability");
                entity.HasKey(e => e.Id);
                entity.Property(c => c.Id).ValueGeneratedOnAdd(); //autogenerada
                entity.Property(e => e.DoctorId).IsRequired();
                entity.Property(e => e.DayOfWeek).IsRequired();
                entity.Property(e => e.StartTime).IsRequired();
                entity.Property(e => e.EndTime).IsRequired();
                entity.Property(e => e.DurationMinutes).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
            });
        }

    }
}
