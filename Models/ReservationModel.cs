namespace Book.App.Models;

public class ReservationModel
{
    public int Id { get; set; }
    public UserModel User { get; set; }
    public TourModel Tour { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; }
}