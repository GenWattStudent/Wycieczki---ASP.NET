using Book.App.Models;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Repositories;

public class ReservationRepository : Repository<ReservationModel>, IReservationRepository
{
    public ReservationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

}