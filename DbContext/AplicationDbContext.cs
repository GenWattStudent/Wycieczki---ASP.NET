using Book.App.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<TourModel> Tours { get; set; }
    public DbSet<UserModel> Users { get; set; }
    public DbSet<WaypointModel> Waypoints { get; set; }
    public DbSet<ImageModel> Images { get; set; }
    public DbSet<PreferencesModel> Preferences { get; set; }
    public DbSet<ContactModel> Contacts { get; set; }
    public DbSet<AddressModel> Addresses { get; set; }
    public DbSet<ReservationModel> Reservations { get; set; }
    public DbSet<MealModel> Meals { get; set; }
    public DbSet<TravelAgencyModel> TravelAgencies { get; set; }
    public DbSet<CommentModel> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReservationModel>()
            .HasOne(r => r.TravelAgency)
            .WithMany(t => t.Reservations)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TourModel>()
            .HasOne(t => t.TravelAgency)
            .WithMany(ta => ta.Tours)
            .HasForeignKey(t => t.TravelAgencyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}