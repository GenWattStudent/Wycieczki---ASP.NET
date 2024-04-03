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
}