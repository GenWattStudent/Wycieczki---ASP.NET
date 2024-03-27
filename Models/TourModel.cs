using System.ComponentModel.DataAnnotations;
using Book.App.Validators;

namespace Book.App.Models;

public class TourModel : BaseEntity
{
    [Required]
    [StringLength(100)]
    [MinLength(3)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [StringLength(500)]
    [MinLength(3)]
    public string Description { get; set; } = string.Empty;
    public List<ImageModel> Images { get; set; } = new();
    [Required]
    [Range(1, 10000)]
    [DataType(DataType.Currency)]
    [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
    public decimal Price { get; set; }
    public List<UserModel> Users { get; set; } = new();
    [Required]
    [Range(1, 100)]
    public int MaxUsers { get; set; }
    public DateTime StartDate { get; set; }
    [DateGreaterThan("StartDate", ErrorMessage = "End date must be greater than start date.")]
    // [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime EndDate { get; set; }
    public List<WaypointModel> Waypoints { get; set; } = new();
    public List<ReservationModel> Reservations { get; set; } = new();

    public TourModel(AddTourModel addTourModel)
    {
        Name = addTourModel.Name;
        Description = addTourModel.Description;
        Price = addTourModel.Price;
        StartDate = addTourModel.StartDate;
        EndDate = addTourModel.EndDate;
        MaxUsers = addTourModel.MaxUsers;
    }

    public TourModel(EditTourModel editTourModel)
    {
        Id = editTourModel.Id;
        Name = editTourModel.Name;
        Description = editTourModel.Description;
        Price = editTourModel.Price;
        StartDate = editTourModel.StartDate;
        EndDate = editTourModel.EndDate;
        MaxUsers = editTourModel.MaxUsers;
    }

    public TourModel EditTour(TourModel tourModel)
    {
        Name = tourModel.Name;
        Description = tourModel.Description;
        Price = tourModel.Price;
        StartDate = tourModel.StartDate;
        EndDate = tourModel.EndDate;
        MaxUsers = tourModel.MaxUsers;
        return this;
    }

    public TourModel()
    {
    }
}