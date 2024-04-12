using Book.App.Models;

namespace Book.App.Services;

public interface IAgencyService
{
    Task<TravelAgencyModel> CreateAsync(TravelAgencyModel travelAgencyModel, int userId);
    Task UpdateAsync(TravelAgencyModel travelAgencyModel);

    Task<List<TravelAgencyModel>> GetNotAcceptedAsync();
    Task<List<TravelAgencyModel>> GetAllByUserIdAsync(string userId);
    Task<TravelAgencyModel> GetByIdAsync(int id);
    Task<List<TravelAgencyModel>> GetAllAsync();

    Task AcceptAsync(int id, string reason);
    Task RejectAsync(int id, string reason);
    Task DeleteAsync(int id, int userId);
    Task LeaveAsync(int userId);
    Task PromoteAsync(int userId, int currentUserId, Role role);

    Task<bool> HasUserAgencyAsync(int userId);
}