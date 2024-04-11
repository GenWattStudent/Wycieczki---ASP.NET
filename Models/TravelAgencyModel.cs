namespace Book.App.Models;


public class TravelAgencyModel : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<TourModel> Tours { get; set; } = new();
    public List<UserModel> Users { get; set; } = new();
    public List<ReservationModel> Reservations { get; set; } = new();
    public List<ImageModel> Images { get; set; } = new();
    public string? VideoPath { get; set; }
    public string? LogoPath { get; set; }

    public int AddressId { get; set; }
    public AddressModel Address { get; set; } = new();

    public bool IsAccepted { get; set; } = false;
    public string Reason { get; set; } = string.Empty;

    public List<CommentModel> Comments { get; set; } = new();
}


public class CommentModel : BaseEntity
{
    public string Text { get; set; } = string.Empty;
    public UserModel User { get; set; }
    public TourModel? Tour { get; set; }
    public TravelAgencyModel? TravelAgency { get; set; }
    public DateTime Date { get; set; }
    public DateTime UpdateDate { get; set; }
    public int Rating { get; set; }
}