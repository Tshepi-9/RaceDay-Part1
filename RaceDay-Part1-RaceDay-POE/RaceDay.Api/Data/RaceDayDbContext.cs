using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
using ModelsRoute = RaceDay.Api.Models.Route;   // add this alias

namespace RaceDay.Api.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(
            DbContextOptions<RaceDayDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;

        public DbSet<ModelsRoute> Routes { get; set; } = null!;    // use alias

        public DbSet<Event> Events { get; set; } = null!;

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<Enrolment> Enrolments { get; set; } = null!;

        public DbSet<Result> Results { get; set; } = null!;

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasMaxLength(20);

            modelBuilder.Entity<User>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Users_Role",
                    "[Role] IN ('Organiser', 'Participant')"));

            // Routes
            modelBuilder.Entity<ModelsRoute>()    // use alias
                .Property(r => r.ElevationGain)
                .HasColumnType("decimal(8,2)");

            // Events
            modelBuilder.Entity<Event>()
                .Property(e => e.Distance)
                .HasColumnType("decimal(6,2)");

            modelBuilder.Entity<Event>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSDATETIME()");

            modelBuilder.Entity<Event>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Events_Distance",
                    "[Distance] > 0"));

            modelBuilder.Entity<Event>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Events_EventType",
                    "[EventType] IN ('Run', 'Walk', 'Cycle')"));

            // One organiser can create many events.
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organiser)
                .WithMany(u => u.OrganisedEvents)
                .HasForeignKey(e => e.OrganiserID)
                .OnDelete(DeleteBehavior.Restrict);

            // A route can be used by many events.
            // An event may have no route.
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Route)
                .WithMany(r => r.Events)
                .HasForeignKey(e => e.RouteID)
                .OnDelete(DeleteBehavior.SetNull);

            // Categories
            modelBuilder.Entity<Category>()
                .Property(c => c.CategoryDistance)
                .HasColumnType("decimal(6,2)");

            modelBuilder.Entity<Category>()
                .HasIndex(c => new
                {
                    c.EventID,
                    c.CategoryName
                })
                .IsUnique();

            modelBuilder.Entity<Category>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Categories_AgeRange",
                        "[MinimumAge] IS NULL OR [MaximumAge] IS NULL OR [MinimumAge] <= [MaximumAge]");

                    t.HasCheckConstraint(
                        "CK_Categories_MinimumAge",
                        "[MinimumAge] IS NULL OR [MinimumAge] >= 0");

                    t.HasCheckConstraint(
                        "CK_Categories_MaximumAge",
                        "[MaximumAge] IS NULL OR [MaximumAge] >= 0");

                    t.HasCheckConstraint(
                        "CK_Categories_Distance",
                        "[CategoryDistance] IS NULL OR [CategoryDistance] > 0");
                });

            // One event can have many categories.
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Event)
                .WithMany(e => e.Categories)
                .HasForeignKey(c => c.EventID)
                .OnDelete(DeleteBehavior.Restrict);

            // Enrolments
            modelBuilder.Entity<Enrolment>()
                .Property(e => e.EnrolmentDate)
                .HasDefaultValueSql("SYSDATETIME()");

            modelBuilder.Entity<Enrolment>()
                .Property(e => e.Status)
                .HasDefaultValue("Confirmed");

            modelBuilder.Entity<Enrolment>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Enrolments_Status",
                    "[Status] IN ('Pending', 'Confirmed', 'Cancelled')"));

            modelBuilder.Entity<Enrolment>()
                .HasIndex(e => new
                {
                    e.ParticipantID,
                    e.EventID
                })
                .IsUnique();

            // One participant can have many enrolments.
            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Participant)
                .WithMany(u => u.ParticipantEnrolments)
                .HasForeignKey(e => e.ParticipantID)
                .OnDelete(DeleteBehavior.Restrict);

            // One event can have many enrolments.
            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Event)
                .WithMany(e => e.Enrolments)
                .HasForeignKey(e => e.EventID)
                .OnDelete(DeleteBehavior.Restrict);

            // One category can be selected for many enrolments.
            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Enrolments)
                .HasForeignKey(e => e.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // Results
            modelBuilder.Entity<Result>()
                .Property(r => r.RecordedAt)
                .HasDefaultValueSql("SYSDATETIME()");

            modelBuilder.Entity<Result>()
                .HasIndex(r => r.EnrolmentID)
                .IsUnique();

            modelBuilder.Entity<Result>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Results_FinishingPosition",
                    "[FinishingPosition] IS NULL OR [FinishingPosition] > 0"));

            // One enrolment can have zero or one result.
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrolment)
                .WithOne(e => e.Result)
                .HasForeignKey<Result>(r => r.EnrolmentID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}