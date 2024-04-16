using Book.App.Models;
using Book.App.ViewModels;

namespace Book.App.Services;

public interface IAgencyService
{
    Task<TravelAgencyModel> CreateAsync(CreateAgencyViewModel travelAgencyModel, int userId);
    Task UpdateAsync(CreateAgencyViewModel travelAgencyModel);

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
    Task<bool> IsUserInAgencyAsync(int userId, int agencyId);

    Task DeleteLogoAsync(int id);
    Task DeleteVideoAsync(int id);
}